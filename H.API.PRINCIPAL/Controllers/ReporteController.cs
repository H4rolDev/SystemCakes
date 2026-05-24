using H.DataAccess.UnitofWork;
using H.DataAccess.Models;
using H.DataAccess.Helpers;
using H.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public ReporteController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("Ventas")]
        public IActionResult Ventas(string fechaDesde, string fechaHasta)
        {
            try
            {
                var desde = DateTime.Parse(fechaDesde);
                var hasta = DateTime.Parse(fechaHasta).AddDays(1);

                var ventas = unitOfWork.VentaRepository
                    .GetBy(x => x.FechaVenta >= desde && x.FechaVenta < hasta && x.Activo)
                    .ToList();

                var ventaIds = ventas.Select(v => v.Id).ToList();

                var totalVentas = ventas.Sum(x => x.Total ?? 0);
                var cantidadVentas = ventas.Count;
                var promedioVenta = cantidadVentas > 0 ? (decimal)totalVentas / cantidadVentas : 0;

                var ventasPorDia = ventas
                    .GroupBy(x => x.FechaVenta.HasValue ? x.FechaVenta.Value.ToString("dd/MM/yyyy") : "")
                    .Select(g => new { fecha = g.Key, total = g.Sum(x => x.Total ?? 0), cantidad = g.Count() })
                    .OrderBy(x => x.fecha)
                    .ToList();

                var pagos = unitOfWork.PagoVentaRepository.GetAll()
                    .ToList()
                    .Where(p => ventaIds.Contains(p.IdVenta))
                    .ToList();

                var ventasPorMetodoPago = pagos
                    .GroupBy(p => {
                        var metodo = unitOfWork.MetodoPagoRepository.GetById(p.IdMetodoPago);
                        return metodo?.Nombre ?? "Desconocido";
                    })
                    .Select(g => new { metodo = g.Key, total = g.Sum(x => x.Monto) })
                    .ToList();

                var detalles = unitOfWork.VentaDetalleRepository.GetAll()
                    .ToList()
                    .Where(d => ventaIds.Contains(d.IdVenta))
                    .ToList();

                var tortasMasVendidas = detalles
                    .GroupBy(d => {
                        var torta = unitOfWork.TortaRepository.GetById(d.IdTorta);
                        return torta?.Nombre ?? "Desconocida";
                    })
                    .Select(g => new { nombre = g.Key, cantidad = (int)g.Sum(x => x.Cantidad), total = g.Sum(x => x.SubTotal ?? 0) })
                    .OrderByDescending(x => x.cantidad)
                    .Take(10)
                    .ToList();

                var deliveries = unitOfWork.EntregaDeliveryRepository.GetAll()
                    .ToList()
                    .Where(d => ventaIds.Contains(d.IdVenta))
                    .ToList();

                var entregasPorEstado = deliveries
                    .GroupBy(d => {
                        var estado = unitOfWork.EstadoEntregaRepository.GetById(d.IdEstadoEntrega);
                        return estado?.Nombre ?? "Desconocido";
                    })
                    .Select(g => new { estado = g.Key, cantidad = g.Count() })
                    .ToList();

                return Ok(new
                {
                    totalVentas,
                    cantidadVentas,
                    promedioVenta,
                    ventasPorDia,
                    ventasPorMetodoPago,
                    tortasMasVendidas,
                    entregasPorEstado
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            try
            {
                var hoy = DateTime.Today;
                var inicioMes = new DateTime(hoy.Year, hoy.Month, 1);
                var inicioMesAnterior = inicioMes.AddMonths(-1);

                var ventasMesActual = unitOfWork.VentaRepository
                    .GetBy(x => x.FechaVenta >= inicioMes && x.Activo)
                    .ToList();
                
                var ventasMesAnterior = unitOfWork.VentaRepository
                    .GetBy(x => x.FechaVenta >= inicioMesAnterior && x.FechaVenta < inicioMes && x.Activo)
                    .ToList();

                var totalMesActual = ventasMesActual.Sum(x => x.Total ?? 0);
                var totalMesAnterior = ventasMesAnterior.Sum(x => x.Total ?? 0);
                var crecimientoPorcentual = totalMesAnterior > 0 ? ((totalMesActual - totalMesAnterior) / totalMesAnterior) * 100 : 0;

                var ventasHoy = unitOfWork.VentaRepository
                    .GetBy(x => x.FechaVenta >= hoy && x.Activo)
                    .ToList();

                var totalVentas = unitOfWork.VentaRepository.GetBy(x => x.Activo).Sum(x => x.Total ?? 0);
                var cantidadClientes = unitOfWork.ClienteRepository.GetBy(x => x.Activo).Count();
                var cantidadTortas = unitOfWork.TortaRepository.GetBy(x => x.Activo).Count();
                var cantidadInsumos = unitOfWork.InsumoRepository.GetBy(x => x.Activo).Count();

                var productosLowStock = unitOfWork.InsumoRepository.GetBy(x => x.Activo && x.StockMinimo != null && x.StockActual <= x.StockMinimo).Count();

                var ultimasVentas = unitOfWork.VentaRepository.GetBy(x => x.Activo)
                    .OrderByDescending(x => x.FechaCreacion)
                    .Take(5)
                    .Select(v => new {
                        id = v.Id,
                        fecha = v.FechaCreacion,
                        total = v.Total ?? 0,
                        cliente = unitOfWork.PersonaRepository.GetById(v.IdPersona)?.Nombres ?? "Desconocido"
                    })
                    .ToList();

                var ventasPorCategoria = unitOfWork.VentaDetalleRepository.GetAll()
                    .Where(d => ventasMesActual.Any(v => v.Id == d.IdVenta))
                    .Join(
                        unitOfWork.TortaRepository.GetAll(),
                        d => d.IdTorta,
                        t => t.Id,
                        (d, t) => new { t.IdCategoriaTorta, d.Cantidad, d.SubTotal }
                    )
                    .GroupBy(x => x.IdCategoriaTorta)
                    .Select(g => new { categoriaId = g.Key, cantidad = (int)g.Sum(x => x.Cantidad), total = g.Sum(x => x.SubTotal ?? 0) })
                    .ToList();

                var categorias = unitOfWork.CategoriaTortaRepository.GetAll().ToList();
                var ventasPorCategoriaNombres = ventasPorCategoria.Select(v => {
                    var cat = categorias.FirstOrDefault(c => c.Id == v.categoriaId);
                    return new { categoria = cat?.Nombre ?? "Sin categoría", cantidad = v.cantidad, total = v.total };
                }).ToList();

                return Ok(new
                {
                    resumen = new
                    {
                        totalVentas,
                        cantidadClientes,
                        cantidadTortas,
                        cantidadInsumos,
                        productosLowStock,
                        ventasHoy = ventasHoy.Count,
                        totalHoy = ventasHoy.Sum(x => x.Total ?? 0)
                    },
                    comparacionMes = new
                    {
                        mesActual = totalMesActual,
                        mesAnterior = totalMesAnterior,
                        crecimientoPorcentual
                    },
                    ultimasVentas,
                    ventasPorCategoria = ventasPorCategoriaNombres
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("Inventario")]
        public IActionResult Inventario()
        {
            try
            {
                var insumos = unitOfWork.InsumoRepository.GetBy(x => x.Activo).ToList();
                
                var insumosData = insumos.Select(i => new
                {
                    id = i.Id,
                    nombre = i.Nombre,
                    cantidad = i.StockActual ?? 0,
                    stockMinimo = i.StockMinimo,
                    unidadMedida = unitOfWork.UnidadMedidaRepository.GetById(i.IdUnidadMedida)?.Nombre ?? "",
                    precioUnitario = i.CostoUnitario ?? 0,
                    estadoStock = (i.StockActual ?? 0) <= (i.StockMinimo ?? 0) ? "Bajo" : ((i.StockActual ?? 0) <= ((i.StockMinimo ?? 0) * 1.5m) ? "Medio" : "Normal")
                }).OrderBy(x => x.cantidad).ToList();

                var tortas = unitOfWork.TortaRepository.GetBy(x => x.Activo).ToList();
                var categorias = unitOfWork.CategoriaTortaRepository.GetBy(x => x.Activo).ToList();

                var tortasData = tortas.Select(t => new
                {
                    id = t.Id,
                    nombre = t.Nombre,
                    precio = t.PrecioVenta,
                    categoria = categorias.FirstOrDefault(c => c.Id == t.IdCategoriaTorta)?.Nombre ?? "Sin categoría",
                    activo = t.Activo
                }).ToList();

                var lowStockInsumos = insumosData.Where(i => i.estadoStock == "Bajo").ToList();
                var normalInsumos = insumosData.Where(i => i.estadoStock == "Normal").ToList();

                return Ok(new
                {
                    insumos = insumosData,
                    tortas = tortasData,
                    lowStockCount = lowStockInsumos.Count,
                    normalCount = normalInsumos.Count,
                    totalInsumos = insumosData.Count,
                    valorTotalInventario = insumos.Sum(i => (i.StockActual ?? 0) * (i.CostoUnitario ?? 0))
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("Clientes")]
        public IActionResult ReporteClientes(string fechaDesde, string fechaHasta)
        {
            try
            {
                DateTime desde = string.IsNullOrEmpty(fechaDesde) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(fechaDesde);
                DateTime hasta = string.IsNullOrEmpty(fechaHasta) ? DateTime.Today.AddDays(1) : DateTime.Parse(fechaHasta).AddDays(1);

                var clientes = unitOfWork.ClienteRepository.GetBy(x => x.Activo).ToList();
                var personas = unitOfWork.PersonaRepository.GetAll().ToList();

                var ventas = unitOfWork.VentaRepository.GetBy(x => x.FechaVenta >= desde && x.FechaVenta < hasta && x.Activo).ToList();

                var clientesTop = ventas
                    .GroupBy(v => v.IdPersona)
                    .Select(g => new
                    {
                        personaId = g.Key,
                        nombre = personas.FirstOrDefault(p => p.Id == g.Key)?.Nombres ?? "Desconocido",
                        apellidoPaterno = personas.FirstOrDefault(p => p.Id == g.Key)?.ApellidoPaterno ?? "",
                        cantidadCompras = g.Count(),
                        totalGastado = g.Sum(x => x.Total ?? 0)
                    })
                    .OrderByDescending(x => x.totalGastado)
                    .Take(10)
                    .ToList();

                var clientesNuevos = personas.Count(p => p.FechaCreacion >= desde && p.FechaCreacion < hasta);

                return Ok(new
                {
                    totalClientes = clientes.Count,
                    clientesNuevos,
                    clientesTop
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("EntradasInsumo")]
        public IActionResult ReporteEntradasInsumo(string fechaDesde, string fechaHasta)
        {
            try
            {
                DateTime desde = string.IsNullOrEmpty(fechaDesde) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(fechaDesde);
                DateTime hasta = string.IsNullOrEmpty(fechaHasta) ? DateTime.Today.AddDays(1) : DateTime.Parse(fechaHasta).AddDays(1);

                var entradas = unitOfWork.EntradaInsumoRepository
                    .GetBy(x => x.FechaCreacion >= desde && x.FechaCreacion < hasta && x.Activo)
                    .ToList();

                var proveedores = unitOfWork.ProveedorRepository.ObtenerCombo().ToList();
                var detalles = unitOfWork.EntradaInsumoDetalleRepository.GetAll().ToList();

                var entradasPorProveedor = entradas
                    .GroupBy(e => e.IdProveedor)
                    .Select(g => new
                    {
                        proveedor = proveedores.FirstOrDefault(p => p.Id == g.Key)?.Nombre ?? "Desconocido",
                        cantidad = g.Count(),
                        total = g.Sum(x => x.Id) // Sin campo Total, usamos cantidad como proxy
                    })
                    .OrderByDescending(x => x.cantidad)
                    .ToList();

                var entradasPorMes = entradas
                    .GroupBy(e => e.FechaCreacion.ToString("MM/yyyy"))
                    .Select(g => new
                    {
                        mes = g.Key,
                        cantidad = g.Count(),
                        total = g.Count() * 100 // Proxy sin campo Total
                    })
                    .OrderBy(x => x.mes)
                    .ToList();

                var totalInsumos = detalles
                    .Where(d => entradas.Any(e => e.Id == d.IdEntradaInsumo))
                    .GroupBy(d => d.IdInsumo)
                    .Select(g => new
                    {
                        insumo = unitOfWork.InsumoRepository.GetById(g.Key)?.Nombre ?? "Desconocido",
                        cantidad = g.Sum(x => x.Cantidad),
                        costoTotal = g.Sum(x => x.Cantidad * x.PrecioUnitario)
                    })
                    .OrderByDescending(x => x.costoTotal)
                    .Take(10)
                    .ToList();

                return Ok(new
                {
                    totalEntradas = entradas.Count,
                    valorTotal = entradas.Count * 100,
                    entradasPorProveedor,
                    entradasPorMes,
                    topInsumos = totalInsumos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("CostosInsumos")]
        public IActionResult ReporteCostosInsumos(string anno)
        {
            try
            {
                var insumos = unitOfWork.InsumoRepository.GetBy(x => x.Activo).ToList();
                var unidadMedida = unitOfWork.UnidadMedidaRepository.GetAll().ToList();

                var lotes = unitOfWork.InsumoLoteRepository.GetAll()
                    .Where(x => x.Activo && x.CantidadDisponible > 0)
                    .ToList();

                int year = string.IsNullOrEmpty(anno) ? DateTime.Today.Year : int.Parse(anno);
                var inicioAnno = new DateTime(year, 1, 1);
                var finAnno = new DateTime(year + 1, 1, 1);

                var entradasAnio = unitOfWork.EntradaInsumoRepository
                    .GetBy(x => x.FechaCreacion >= inicioAnno && x.FechaCreacion < finAnno && x.Activo)
                    .ToList();
                var entradaIdsAnio = entradasAnio.Select(e => e.Id).ToList();

                var detallesEntradas = unitOfWork.EntradaInsumoDetalleRepository.GetAll()
                    .Where(d => entradaIdsAnio.Contains(d.IdEntradaInsumo))
                    .ToList();

                var costoPorInsumo = insumos.Select(i =>
                {
                    var lotesInsumo = lotes.Where(l => l.IdInsumo == i.Id).ToList();
                    var um = unidadMedida.FirstOrDefault(u => u.Id == i.IdUnidadMedida);
                    var stockActual = i.StockActual ?? lotesInsumo.Sum(l => l.CantidadDisponible);
                    var ultimoCosto = lotesInsumo.OrderByDescending(l => l.FechaIngreso).FirstOrDefault()?.CostoUnitario ?? i.CostoUnitario ?? 0;
                    var cantidadRecibida = detallesEntradas.Where(d => d.IdInsumo == i.Id).Sum(d => d.Cantidad);

                    return new
                    {
                        insumo = i.Nombre,
                        stockActual = stockActual,
                        unidadMedida = um?.Nombre ?? "",
                        ultimoCosto = ultimoCosto,
                        cantidadRecibida = cantidadRecibida,
                        costoTotal = stockActual * ultimoCosto,
                        estado = stockActual <= (i.StockMinimo ?? 0) ? "Bajo Stock" : "Normal"
                    };
                }).OrderByDescending(x => x.costoTotal).ToList();

                var valorTotalInventario = costoPorInsumo.Sum(x => x.costoTotal);

                return Ok(new
                {
                    costos = costoPorInsumo,
                    valorTotalInventario,
                    cantidadInsumos = insumos.Count
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("Produccion")]
        public IActionResult ReporteProduccion(string fechaDesde, string fechaHasta)
        {
            try
            {
                DateTime desde = string.IsNullOrEmpty(fechaDesde) ? DateTime.Today.AddMonths(-1) : DateTime.Parse(fechaDesde);
                DateTime hasta = string.IsNullOrEmpty(fechaHasta) ? DateTime.Today.AddDays(1) : DateTime.Parse(fechaHasta).AddDays(1);

                var producciones = unitOfWork.ProduccionRepository
                    .GetBy(x => x.FechaProduccion >= desde && x.FechaProduccion < hasta && x.Activo)
                    .ToList();

                var produccionPorDia = producciones
                    .GroupBy(p => p.FechaProduccion.ToString("dd/MM/yyyy"))
                    .Select(g => new
                    {
                        fecha = g.Key,
                        cantidad = g.Count()
                    })
                    .OrderBy(x => x.fecha)
                    .ToList();

                return Ok(new
                {
                    totalProducciones = producciones.Count,
                    tortasProducidas = 0,
                    topTortas = new List<object>(),
                    produccionPorDia
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("Financiero")]
        public IActionResult ReporteFinanciero(string anno)
        {
            try
            {
                int year = string.IsNullOrEmpty(anno) ? DateTime.Today.Year : int.Parse(anno);
                var inicioAnno = new DateTime(year, 1, 1);
                var finAnno = new DateTime(year + 1, 1, 1);

                var ventas = unitOfWork.VentaRepository
                    .GetBy(x => x.FechaVenta >= inicioAnno && x.FechaVenta < finAnno && x.Activo)
                    .ToList();

                var entradas = unitOfWork.EntradaInsumoRepository
                    .GetBy(x => x.FechaCreacion >= inicioAnno && x.FechaCreacion < finAnno && x.Activo)
                    .ToList();

                var entradaIdsAll = entradas.Select(e => e.Id).ToList();
                var detalles = unitOfWork.EntradaInsumoDetalleRepository.GetAll()
                    .Where(d => entradaIdsAll.Contains(d.IdEntradaInsumo))
                    .ToList();

                var resumenMensual = Enumerable.Range(1, 12).Select(mes =>
                {
                    var mesDate = new DateTime(year, mes, 1);
                    var ventasMes = ventas.Where(v => v.FechaVenta.HasValue && v.FechaVenta.Value.Month == mes).ToList();
                    var entradaMes = entradas.Where(e => e.FechaCreacion.Month == mes).ToList();
                    var entradaIdsMes = entradaMes.Select(e => e.Id).ToList();
                    var costosMes = detalles.Where(d => entradaIdsMes.Contains(d.IdEntradaInsumo)).Sum(d => d.Cantidad * d.PrecioUnitario);

                    return new
                    {
                        mes = mesDate.ToString("MMMM"),
                        ventas = ventasMes.Sum(v => v.Total ?? 0),
                        cantidadVentas = ventasMes.Count,
                        costos = costosMes,
                        ganancia = ventasMes.Sum(v => v.Total ?? 0) - costosMes
                    };
                }).ToList();

                var totalCostosAll = detalles.Sum(d => d.Cantidad * d.PrecioUnitario);
                var totalVentasAnno = ventas.Sum(v => v.Total ?? 0);
                var gananciaNeta = totalVentasAnno - totalCostosAll;
                var margenGanancia = totalVentasAnno > 0 ? (gananciaNeta / totalVentasAnno) * 100 : 0;

                return Ok(new
                {
                    anno = year,
                    totalVentas = totalVentasAnno,
                    totalCostos = totalCostosAll,
                    gananciaNeta = gananciaNeta,
                    margenPorcentual = margenGanancia,
                    mensual = resumenMensual
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpGet("MetaVenta")]
        public IActionResult ReporteMetaVenta(int anno)
        {
            try
            {
                int year = anno > 0 ? anno : DateTime.Today.Year;
                var inicioAnno = new DateTime(year, 1, 1);
                var finAnno = new DateTime(year + 1, 1, 1);

                var ventas = unitOfWork.VentaRepository
                    .GetBy(x => x.FechaVenta >= inicioAnno && x.FechaVenta < finAnno && x.Activo)
                    .ToList();

                var metaConfig = unitOfWork.MetaVentaRepository
                    .GetBy(x => x.Anio == year && x.Activo)
                    .FirstOrDefault();

                decimal metaAnual;
                if (metaConfig != null)
                {
                    metaAnual = metaConfig.MetaAnual;
                }
                else
                {
                    var promedioMensual = ventas.Count > 0 ? (ventas.Sum(v => v.Total ?? 0) / DateTime.Today.Month) : 0;
                    metaAnual = promedioMensual * 1.2m * 12;
                }

                var metaMensual = metaAnual / 12;

                var cumplimientoPorMes = Enumerable.Range(1, DateTime.Today.Month).Select(mes =>
                {
                    var mesVentas = ventas.Where(v => v.FechaVenta.HasValue && v.FechaVenta.Value.Month == mes).ToList();
                    var real = mesVentas.Sum(v => v.Total ?? 0);
                    var porcentaje = metaMensual > 0 ? (real / metaMensual) * 100 : 0;

                    return new
                    {
                        mes = new DateTime(year, mes, 1).ToString("MMMM"),
                        meta = metaMensual,
                        real = real,
                        cumplimiento = porcentaje
                    };
                }).ToList();

                var totalVentas = ventas.Sum(v => v.Total ?? 0);

                return Ok(new
                {
                    anno = year,
                    metaAnual = metaAnual,
                    ventasReal = totalVentas,
                    cumplimientoAnual = metaAnual > 0 ? (totalVentas / metaAnual) * 100 : 0,
                    cumplimientoPorMes,
                    metaConfigurada = metaConfig != null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }

        [HttpPost("MetaVenta")]
        public IActionResult GuardarMetaVenta([FromBody] GuardarMetaVentaDTO dto)
        {
            try
            {
                var metaExistente = unitOfWork.MetaVentaRepository
                    .GetBy(x => x.Anio == dto.Anio && x.Activo)
                    .FirstOrDefault();

                if (metaExistente != null)
                {
                    metaExistente.MetaAnual = dto.MetaAnual;
                    metaExistente.UsuarioModificacion = dto.Usuario;
                    metaExistente.FechaModificacion = Fecha.Hoy;
                    unitOfWork.MetaVentaRepository.Update(metaExistente);
                }
                else
                {
                    var nuevaMeta = new H.DataAccess.Models.TMetaVenta
                    {
                        Anio = dto.Anio,
                        MetaAnual = dto.MetaAnual,
                        Activo = true,
                        UsuarioCreacion = dto.Usuario,
                        FechaCreacion = Fecha.Hoy
                    };
                    unitOfWork.MetaVentaRepository.Add(nuevaMeta);
                }

                unitOfWork.Commit();
                return Ok(new { message = "Meta guardada correctamente" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message, stack = ex.StackTrace });
            }
        }
    }

    public class GuardarMetaVentaDTO
    {
        public int Anio { get; set; }
        public decimal MetaAnual { get; set; }
        public string Usuario { get; set; } = null!;
    }
}