using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using H.DataAccess.Entidades;
using H.DataAccess.UnitofWork;
using H.DTOs;

namespace H.Services
{
    /// <summary>
    /// Conversational ordering service. It deliberately returns structured actions so
    /// the clients do not need to parse commands hidden inside natural language.
    /// </summary>
    public class ChatService : IChatService
    {
        private static readonly ConcurrentDictionary<string, ChatSession> Sessions = new();
        private static readonly string[] StopWords =
        {
            "quiero", "busco", "necesito", "una", "uno", "un", "de", "la", "el", "para",
            "por", "favor", "torta", "tortas", "pastel", "que", "con", "y", "me", "gustaria"
        };

        private readonly IUnitOfWork _unitOfWork;

        public ChatService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public int? GetPersonaId(int userId) => _unitOfWork.UsuarioRepository.GetById(userId)?.IdPersona;

        public ChatResponseDTO Respond(ChatRequestDTO request, bool isAuthenticated, int? authenticatedPersonaId)
        {
            var conversationId = string.IsNullOrWhiteSpace(request.ConversationId)
                ? Guid.NewGuid().ToString("N")
                : request.ConversationId.Trim();
            var sessionKey = authenticatedPersonaId.HasValue
                ? $"persona:{authenticatedPersonaId.Value}"
                : $"guest:{conversationId}";
            var session = Sessions.GetOrAdd(sessionKey, _ => new ChatSession());
            session.LastUsedUtc = DateTime.UtcNow;
            RemoveExpiredSessions();

            var text = request.Message.Trim();
            var normalized = Normalize(text);
            var catalog = ActiveCatalog();
            var response = NewResponse(conversationId, isAuthenticated, authenticatedPersonaId);

            if (normalized.Contains("reiniciar") || normalized.Contains("nueva conversacion"))
            {
                session.Reset();
                response.Intent = "conversation_reset";
                response.Message = "Listo, empezamos de nuevo. ¿Qué torta o sabor estás buscando?";
                response.Actions.Add(Action("show_catalog", "Ver tortas disponibles"));
                return Finish(response, session);
            }

            var isGreeting = HasAny(normalized, "hola", "buenos dias", "buenas tardes", "buenas noches", "buenas", "hi", "hello", "que tal", "como estas", "que onda", "saludos");
            if (isGreeting)
            {
                if (session.Cart.Count > 0)
                {
                    response.Intent = "greeting_with_cart";
                    response.Message = $"¡Hola de nuevo! 🎂 Tienes {session.Cart.Count} torta{(session.Cart.Count > 1 ? "s" : "")} en tu carrito. ¿Deseas agregar otra, revisar tu carrito o continuar con el pedido?";
                    response.Actions.Add(Action("cart_summary", "Ver mi carrito"));
                    response.Actions.Add(Action("continue_shopping", "Agregar otra torta"));
                    response.Actions.Add(Action("confirm_checkout", "Continuar con el pedido"));
                }
                else
                {
                    response.Intent = "greeting";
                    response.Message = "¡Hola! 👋 Soy Yani, tu asistente de tortas. ¿Qué se te antoja hoy? Puedo recomendarte según la ocasión o ayudarte a encontrar una torta específica.";
                    response.Actions.Add(Action("show_catalog", "Ver tortas disponibles"));
                    response.Actions.Add(Action("recommend_birthday", "Para un cumpleaños"));
                    response.Actions.Add(Action("recommend_wedding", "Para una boda o evento"));
                    response.Actions.Add(Action("recommend_best", "Las más vendidas"));
                }
                return Finish(response, session);
            }

            var matches = ResolveProducts(text, normalized, catalog, session.LastProducts);
            var explicitAdd = HasAny(normalized, "agrega", "agregar", "anade", "añade", "anadir", "añadir", "al carrito", "carrito", "llevo", "llevame", "comprar");
            var cartQuestion = HasAny(normalized, "mi carrito", "carrito", "resumen");
            var checkoutQuestion = HasAny(normalized, "checkout", "finalizar", "terminar compra", "continuar con el pedido", "pagar ahora");
            var confirmed = HasAny(normalized, "si", "sí", "confirmo", "confirmar", "continuar", "finalizar") &&
                (session.PendingCheckout || session.Cart.Count > 0 || request.HasClientCart);
            var removeRequest = HasAny(normalized, "quita", "quitar", "elimina", "eliminar", "saca", "sacar");

            if (cartQuestion && !explicitAdd)
            {
                response.Intent = "cart_summary";
                response.Message = BuildCartMessage(session.Cart, request.HasClientCart);
                response.Actions.Add(Action("continue_shopping", "Agregar otra torta"));
                if (session.Cart.Count > 0 || request.HasClientCart)
                    response.Actions.Add(Action("request_checkout", "Continuar con mi pedido"));
                return Finish(response, session);
            }

            if (removeRequest)
            {
                response.Intent = "remove_from_cart";
                if (session.Cart.Count == 0)
                {
                    response.Message = request.HasClientCart
                        ? "Tu carrito tiene productos agregados desde el catálogo. Puedes quitarlos desde el carrito o decirme qué producto deseas retirar."
                        : "Tu carrito conversacional está vacío. ¿Qué torta deseas agregar?";
                }
                else
                {
                    var removed = RemoveCartItem(session, matches.FirstOrDefault()?.Id);
                    response.Message = removed
                        ? "Retiré la torta indicada. ¿Deseas agregar otra o continuar con tu pedido?"
                        : "Dime el nombre de la torta que deseas retirar.";
                }
                return Finish(response, session);
            }

            if (confirmed)
            {
                session.PendingCheckout = false;
                response.Intent = "checkout_start";
                if (isAuthenticated && (session.Cart.Count > 0 || request.HasClientCart))
                {
                    response.Message = "Perfecto. Abriré la confirmación del pedido para que elijas recojo o delivery y completes el pago.";
                    response.Actions.Add(Action("open_checkout", "Continuar al checkout"));
                }
                else if (!isAuthenticated)
                {
                    response.RequiresLoginForCheckout = true;
                    response.Message = "Para registrar tu pedido debes iniciar sesión o crear una cuenta. Tu selección queda lista para continuar después.";
                    response.Actions.Add(Action("open_login", "Iniciar sesión"));
                }
                else
                {
                    response.Message = "Tu carrito está vacío. Primero elijamos una torta.";
                    response.Actions.Add(Action("show_catalog", "Ver tortas disponibles"));
                }
                return Finish(response, session);
            }

            if (checkoutQuestion)
            {
                response.Intent = "checkout_information";
                session.PendingCheckout = session.Cart.Count > 0 || request.HasClientCart;
                response.Message = isAuthenticated
                    ? "Puedo llevarte al checkout cuando confirmes el resumen. ¿Deseas revisar tu carrito o continuar con el pedido?"
                    : "Puedes revisar el catálogo sin iniciar sesión, pero necesitarás iniciar sesión para registrar el pedido.";
                response.Actions.Add(Action("cart_summary", "Revisar carrito"));
                if (session.PendingCheckout)
                    response.Actions.Add(Action("confirm_checkout", "Sí, continuar"));
                return Finish(response, session);
            }

            if (explicitAdd && matches.Count > 0)
            {
                response.Intent = "add_to_cart";
                var quantitiesBefore = session.Cart
                    .GroupBy(item => item.ProductId)
                    .ToDictionary(group => group.Key, group => group.Sum(item => item.Quantity));
                var quantity = ParseQuantity(text);
                if (matches.Count == 1)
                {
                    AddProduct(session, matches[0], quantity, text);
                }
                else
                {
                    foreach (var product in matches.Take(5))
                        AddProduct(session, product, 1, text);
                }

                response.AddedItems = matches.Take(5)
                    .Select(product => session.Cart.LastOrDefault(item => item.ProductId == product.Id))
                    .Where(item => item != null)
                    .Select(item => ToCartItem(item!, Math.Max(1,
                        item!.Quantity - quantitiesBefore.GetValueOrDefault(item.ProductId))))
                    .ToList();
                var firstAdded = response.AddedItems.FirstOrDefault();
                if (firstAdded != null)
                {
                    response.AddToCart = new ChatAddToCartDTO
                    {
                        ProductId = firstAdded.ProductId,
                        ProductName = firstAdded.ProductName,
                        Quantity = firstAdded.Quantity,
                        UnitPrice = firstAdded.UnitPrice,
                        Subtotal = firstAdded.Subtotal
                    };
                }

                response.Message = BuildAddedMessage(session, matches);
                response.Actions.Add(Action("cart_summary", "Ver mi carrito"));
                response.Actions.Add(Action("continue_shopping", "Agregar otra torta"));
                response.Actions.Add(Action("confirm_checkout", "Continuar con el pedido"));
                return Finish(response, session);
            }

            if (matches.Count > 0)
            {
                response.Intent = "recommendation";
                session.LastProducts = matches.Take(5).ToList();
                response.Products = matches.Take(5).Select(ToProduct).ToList();
                response.Message = BuildRecommendationMessage(matches, text);
                response.RequiresClarification = true;
                response.PendingField = "product_or_configuration";
                foreach (var product in matches.Take(3))
                    response.Actions.Add(Action($"select_product_{product.Id}", $"Elegir {product.Nombre}", product.Id));
                response.Actions.Add(Action("show_more_products", "Ver más opciones"));
                return Finish(response, session);
            }

            response.Intent = "recommendation";
            var recommendations = catalog.Where(p => (p.StockDisponible ?? 0) > 0)
                .OrderByDescending(p => p.StockDisponible ?? 0)
                .Take(5)
                .ToList();
            session.LastProducts = recommendations;
            response.Products = recommendations.Select(ToProduct).ToList();
            response.Message = string.IsNullOrWhiteSpace(text)
                ? "Soy Yane. Empecemos por la torta: dime un sabor, tamaño, relleno o nombre de producto. También puedo recomendarte una según tu presupuesto u ocasión."
                : "No encontré una coincidencia exacta. Empecemos por la torta: ¿buscas chocolate, vainilla, cheesecake, tres leches u otra opción? Después afinamos tamaño, relleno y cantidad.";
            response.RequiresClarification = true;
            response.PendingField = "cake";
            foreach (var product in recommendations.Take(3))
                response.Actions.Add(Action($"select_product_{product.Id}", $"Ver {product.Nombre}", product.Id));
            return Finish(response, session);
        }

        private ChatResponseDTO NewResponse(string conversationId, bool authenticated, int? personaId) => new()
        {
            ConversationId = conversationId,
            IsAuthenticated = authenticated,
            RequiresLoginForCheckout = !authenticated,
            OrderStates = GetOrderStates(personaId)
        };

        private ChatResponseDTO Finish(ChatResponseDTO response, ChatSession session)
        {
            response.Cart = session.Cart.Select(item => ToCartItem(item)).ToList();
            return response;
        }

        private List<H.DTOs.TortaListadoDTO> ActiveCatalog() => _unitOfWork.TortaRepository.ObtenerCombo()
            .Where(product => string.Equals(product.Activo, "S", StringComparison.OrdinalIgnoreCase))
            .ToList();

        private List<H.DTOs.TortaListadoDTO> ResolveProducts(string text, string normalized,
            List<H.DTOs.TortaListadoDTO> catalog, List<H.DTOs.TortaListadoDTO> previous)
        {
            var referenced = catalog.Where(product =>
                !string.IsNullOrWhiteSpace(product.Nombre) &&
                normalized.Contains(Normalize(product.Nombre!))).ToList();
            if (referenced.Count > 0) return referenced;

            var ordinal = Regex.Match(normalized, @"\b(primera|segundo|segunda|tercero|tercera|1|2|3)\b");
            if (ordinal.Success && previous.Count > 0)
            {
                var index = ordinal.Groups[1].Value switch
                {
                    "primera" or "1" => 0,
                    "segundo" or "segunda" or "2" => 1,
                    "tercero" or "tercera" or "3" => 2,
                    _ => -1
                };
                return index >= 0 && index < previous.Count ? new List<H.DTOs.TortaListadoDTO> { previous[index] } : new();
            }

            var terms = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Where(term => term.Length > 2 && !StopWords.Contains(term))
                .ToArray();
            if (terms.Length == 0) return new();
            return catalog.Select(product => new
                {
                    Product = product,
                    Score = terms.Sum(term => ScoreTerm(product, term))
                })
                .Where(item => item.Score > 0)
                .OrderByDescending(item => item.Score)
                .ThenByDescending(item => item.Product.StockDisponible ?? 0)
                .Take(5)
                .Select(item => item.Product)
                .ToList();
        }

        private static int ScoreTerm(H.DTOs.TortaListadoDTO product, string term)
        {
            var searchable = Normalize(string.Join(' ', product.Nombre, product.Descripcion,
                product.Cantidades, product.NombreCategoriaTorta));
            if (searchable.Contains(term, StringComparison.Ordinal)) return 2;
            return Similarity(searchable, term) >= 0.78 ? 1 : 0;
        }

        private static double Similarity(string value, string term)
        {
            var words = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.Select(word => Levenshtein(word, term)).DefaultIfEmpty(99)
                .Min(distance => 1d - Math.Min(distance, Math.Max(wordLength(term), 1)) / (double)Math.Max(wordLength(term), 1));
        }

        private static int wordLength(string value) => value.Length;

        private static int Levenshtein(string left, string right)
        {
            var row = Enumerable.Range(0, right.Length + 1).ToArray();
            for (var i = 1; i <= left.Length; i++)
            {
                var previous = row[0];
                row[0] = i;
                for (var j = 1; j <= right.Length; j++)
                {
                    var current = row[j];
                    row[j] = Math.Min(Math.Min(row[j] + 1, row[j - 1] + 1), previous + (left[i - 1] == right[j - 1] ? 0 : 1));
                    previous = current;
                }
            }
            return row[right.Length];
        }

        private void AddProduct(ChatSession session, H.DTOs.TortaListadoDTO product, int quantity, string text)
        {
            var stock = Math.Max(product.StockDisponible ?? 0, 0);
            if (stock == 0) return;
            var safeQuantity = Math.Clamp(quantity, 1, stock);
            var size = ParseSize(text);
            var flavor = ParseChoice(text, "chocolate", "vainilla", "fresa", "zanahoria");
            var filling = ParseChoice(text, "manjar blanco", "manjar", "chocolate", "fresa", "maracuya", "oreo");
            var floors = ParseFloors(text);
            var decorationColor = ParseChoice(text, "rosa pastel", "celeste", "dorado", "blanco perla");
            var message = ExtractMessage(text);
            var unitPrice = CalculatePrice(product, size, flavor, filling, floors, decorationColor);
            var item = session.Cart.FirstOrDefault(x => x.ProductId == product.Id &&
                x.Message == message && x.Size == size && x.Flavor == flavor &&
                x.Filling == filling && x.Floors == floors && x.DecorationColor == decorationColor);
            if (item == null)
            {
                session.Cart.Add(new SessionCartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Nombre ?? "Torta",
                    Quantity = safeQuantity,
                    UnitPrice = unitPrice,
                    ImageUrl = product.ImagenUrl ?? string.Empty,
                    Message = message,
                    Size = size,
                    Flavor = flavor,
                    Filling = filling,
                    Floors = floors,
                    DecorationColor = decorationColor
                });
            }
            else
            {
                item.Quantity = Math.Min(item.Quantity + safeQuantity, stock);
            }
        }

        private static string ExtractMessage(string text)
        {
            var match = Regex.Match(text, "(?:diga|dices|mensaje|que diga)\\s+['\"]?(.+?)['\"]?$", RegexOptions.IgnoreCase);
            return match.Success ? match.Groups[1].Value.Trim() : string.Empty;
        }

        private decimal CalculatePrice(H.DTOs.TortaListadoDTO product, string size,
            string flavor, string filling, int floors, string decorationColor)
        {
            var price = product.PrecioVenta ?? 0;
            var values = new[]
            {
                ("tamanio", size), ("sabor", flavor), ("relleno", filling),
                ("pisos", floors > 1 ? floors.ToString(CultureInfo.InvariantCulture) : string.Empty),
                ("color", decorationColor)
            };
            foreach (var (type, value) in values.Where(item => !string.IsNullOrWhiteSpace(item.Item2)))
            {
                var option = _unitOfWork.TortaOpcionRepository.ObtenerPorClave(product.Id, type, value);
                if (option != null) price += option.PrecioExtra;
            }
            return price;
        }

        private static string ParseSize(string text)
        {
            var normalized = Normalize(text);
            if (Regex.IsMatch(normalized, @"\b(xl|extra grande)\b")) return "XL";
            if (Regex.IsMatch(normalized, @"\b(grande|tamano l|tamanio l)\b")) return "L";
            if (Regex.IsMatch(normalized, @"\b(mediana|tamano m|tamanio m)\b")) return "M";
            if (Regex.IsMatch(normalized, @"\b(pequena|pequeno|tamano s|tamanio s)\b")) return "S";
            return string.Empty;
        }

        private static string ParseChoice(string text, params string[] choices)
        {
            var normalized = Normalize(text);
            return choices.FirstOrDefault(choice => normalized.Contains(Normalize(choice), StringComparison.Ordinal)) ?? string.Empty;
        }

        private static int ParseFloors(string text)
        {
            var normalized = Normalize(text);
            var match = Regex.Match(normalized, @"\b(\d+)\s+pisos?\b");
            return match.Success && int.TryParse(match.Groups[1].Value, out var floors)
                ? Math.Clamp(floors, 1, 5)
                : 1;
        }

        private static bool RemoveCartItem(ChatSession session, int? productId)
        {
            if (!productId.HasValue) return false;
            var item = session.Cart.FirstOrDefault(x => x.ProductId == productId.Value);
            return item != null && session.Cart.Remove(item);
        }

        private static int ParseQuantity(string text)
        {
            var normalized = Normalize(text);
            var word = Regex.Match(normalized, @"\b(una|uno|dos|tres|cuatro|cinco)\s+(?:tortas?|unidades?)\b").Groups[1].Value;
            if (word.Length > 0)
                return word switch { "dos" => 2, "tres" => 3, "cuatro" => 4, "cinco" => 5, _ => 1 };
            var numeric = Regex.Match(normalized, @"\b(\d+)\s*(?:tortas?|unidades?)\b");
            return numeric.Success && int.TryParse(numeric.Groups[1].Value, out var quantity) ? quantity : 1;
        }

        private static string BuildRecommendationMessage(IEnumerable<H.DTOs.TortaListadoDTO> products, string text)
        {
            var list = products.Take(3).Select((product, index) =>
                $"{index + 1}. {product.Nombre} - S/ {(product.PrecioVenta ?? 0):0.00} " +
                $"({((product.StockDisponible ?? 0) > 0 ? "Disponible" : "Sin stock")})");
            return $"Para ayudarte con la torta, encontré estas opciones relacionadas con tu búsqueda:\n{string.Join("\n", list)}\n¿Cuál deseas configurar? Puedo ayudarte con tamaño, relleno, pisos, mensaje y cantidad.";
        }

        private static string BuildAddedMessage(ChatSession session, IEnumerable<H.DTOs.TortaListadoDTO> products)
        {
            var names = string.Join(", ", products.Select(product => product.Nombre).Where(name => !string.IsNullOrWhiteSpace(name)));
            return $"Agregué {names} a tu pedido. Puedes agregar otra torta, modificar el carrito o revisar el resumen antes de continuar.";
        }

        private static string BuildCartMessage(List<SessionCartItem> cart, bool hasClientCart)
        {
            if (cart.Count == 0 && !hasClientCart) return "Todavía no tienes tortas en el carrito. ¿Qué torta deseas agregar?";
            if (cart.Count == 0) return "Tienes productos en tu carrito. Puedes revisarlos y continuar desde el botón del carrito.";
            var lines = cart.Select(item => $"• {item.Quantity}x {item.ProductName}: S/ {item.Subtotal:0.00}");
            return $"Este es el resumen de tu pedido:\n{string.Join("\n", lines)}\nSubtotal: S/ {cart.Sum(item => item.Subtotal):0.00}\n¿Deseas agregar otra torta o continuar?";
        }

        private ChatProductDTO ToProduct(H.DTOs.TortaListadoDTO product)
        {
            var options = _unitOfWork.TortaOpcionRepository.ObtenerPorTorta(product.Id, true)
                .Select(option => new ChatOptionDTO { Type = option.Tipo, Value = option.Valor, ExtraPrice = option.PrecioExtra })
                .ToList();
            return new ChatProductDTO
            {
                Id = product.Id,
                Name = product.Nombre ?? string.Empty,
                Description = product.Descripcion,
                CurrentPrice = product.PrecioVenta,
                StockAvailable = product.StockDisponible ?? 0,
                IsCustomizable = product.EsPersonalizable ?? false,
                ImageUrl = product.ImagenUrl,
                Category = product.NombreCategoriaTorta,
                Options = options
            };
        }

        private static ChatCartItemDTO ToCartItem(SessionCartItem item, int? quantityOverride = null) => new()
        {
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            Quantity = quantityOverride ?? item.Quantity,
            UnitPrice = item.UnitPrice,
            Subtotal = item.UnitPrice * (quantityOverride ?? item.Quantity),
            Size = item.Size,
            Flavor = item.Flavor,
            Filling = item.Filling,
            Floors = item.Floors,
            DecorationColor = item.DecorationColor,
            Message = item.Message,
            ImageUrl = item.ImageUrl
        };

        private static ChatActionDTO Action(string id, string label, int? productId = null, int? quantity = null) => new()
        {
            Id = id,
            Type = id.StartsWith("select_product_") ? "select_product" : id,
            Label = label,
            ProductId = productId,
            Quantity = quantity
        };

        private List<ChatOrderStateDTO> GetOrderStates(int? idPersona)
        {
            if (!idPersona.HasValue || idPersona <= 0) return new();
            return _unitOfWork.VentaRepository.GetBy(sale => sale.IdPersona == idPersona && sale.Activo)
                .OrderByDescending(sale => sale.FechaVenta)
                .Take(5)
                .Select(sale => new ChatOrderStateDTO
                {
                    State = _unitOfWork.EstadoVentaRepository.GetById(sale.IdEstadoVenta)?.Nombre ?? "Pendiente",
                    Explanation = $"Pedido #{sale.Id} por S/ {(sale.Total ?? 0):0.00}. Consulta el historial para ver el detalle y el estado actualizado."
                }).ToList();
        }

        private static bool HasAny(string value, params string[] terms) => terms.Any(term => value.Contains(Normalize(term), StringComparison.Ordinal));

        private static string Normalize(string value)
        {
            var decomposed = value.ToLowerInvariant().Normalize(NormalizationForm.FormD);
            return new string(decomposed.Where(character => CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark).ToArray()).Normalize(NormalizationForm.FormC);
        }

        private static void RemoveExpiredSessions()
        {
            var cutoff = DateTime.UtcNow.AddHours(-12);
            foreach (var item in Sessions.Where(item => item.Value.LastUsedUtc < cutoff))
                Sessions.TryRemove(item.Key, out _);
        }

        private sealed class ChatSession
        {
            public DateTime LastUsedUtc { get; set; } = DateTime.UtcNow;
            public List<H.DTOs.TortaListadoDTO> LastProducts { get; set; } = new();
            public List<SessionCartItem> Cart { get; set; } = new();
            public bool PendingCheckout { get; set; }
            public void Reset()
            {
                LastProducts.Clear();
                Cart.Clear();
                PendingCheckout = false;
            }
        }

        private sealed class SessionCartItem
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public decimal UnitPrice { get; set; }
            public string ImageUrl { get; set; } = string.Empty;
            public string Size { get; set; } = string.Empty;
            public string Flavor { get; set; } = string.Empty;
            public string Filling { get; set; } = string.Empty;
            public int Floors { get; set; } = 1;
            public string DecorationColor { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public decimal Subtotal => UnitPrice * Quantity;
        }
    }
}
