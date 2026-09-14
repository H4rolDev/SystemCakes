namespace H.DTOs;

public class DeliveryConfigurationDTO
{
    public decimal CostoBase { get; set; }
    public decimal CostoPorKilometro { get; set; }
    public decimal LatitudCentro { get; set; }
    public decimal LongitudCentro { get; set; }
    public decimal RadioMaximoKm { get; set; }
    public bool Activo { get; set; }
}
