namespace H.DTOs
{
    public class ChatRequestDTO
    {
        public string Message { get; set; } = string.Empty;
        public string? ConversationId { get; set; }
        public bool HasClientCart { get; set; }
        // Kept for old clients. The API must never trust this value for order data.
        public int? IdPersona { get; set; }
    }

    public class ChatResponseDTO
    {
        public string Message { get; set; } = string.Empty;
        public string Intent { get; set; } = "general";
        public string ConversationId { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public bool RequiresLoginForCheckout { get; set; }
        public bool RequiresClarification { get; set; }
        public string? PendingField { get; set; }
        public List<ChatProductDTO> Products { get; set; } = new();
        public ChatAddToCartDTO? AddToCart { get; set; }
        public List<ChatCartItemDTO> Cart { get; set; } = new();
        public List<ChatCartItemDTO> AddedItems { get; set; } = new();
        public List<ChatActionDTO> Actions { get; set; } = new();
        public List<ChatOrderStateDTO> OrderStates { get; set; } = new();
    }

    public class ChatProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal? CurrentPrice { get; set; }
        public int StockAvailable { get; set; }
        public bool IsCustomizable { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public List<ChatOptionDTO> Options { get; set; } = new();
    }

    public class ChatOptionDTO
    {
        public string Type { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public decimal ExtraPrice { get; set; }
    }

    public class ChatCartItemDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Flavor { get; set; } = string.Empty;
        public string Filling { get; set; } = string.Empty;
        public int Floors { get; set; } = 1;
        public string DecorationColor { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }

    public class ChatActionDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public int? Quantity { get; set; }
    }

    public class ChatAddToCartDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ChatOrderStateDTO
    {
        public string State { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }
}
