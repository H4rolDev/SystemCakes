namespace H.DTOs;

public class TortaOpcionDTO
{
    public int Id { get; set; }
    public int IdTorta { get; set; }
    public string Tipo { get; set; } = null!;
    public string Valor { get; set; } = null!;
    public decimal PrecioExtra { get; set; }
    public string ModoPrecio { get; set; } = "fijo";
    public decimal PrecioPorUnidad { get; set; }
    public bool Obligatorio { get; set; }
    public int? Minimo { get; set; }
    public bool Activo { get; set; }
    public int? Maximo { get; set; }
    public int Orden { get; set; }
}
