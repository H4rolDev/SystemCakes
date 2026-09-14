using H.DataAccess.Models;
using H.DataAccess.UnitofWork;
using H.DTOs;

namespace H.Services;

public static class DeliveryPricingService
{
    public const decimal DefaultBaseCost = 5.00m;
    public const decimal DefaultCostPerKilometer = 1.50m;
    public const decimal DefaultLatitude = -13.53195m;
    public const decimal DefaultLongitude = -71.96746m;
    public const decimal DefaultRadiusKm = 35m;

    public static TConfiguracionDelivery GetOrCreate(IUnitOfWork unitOfWork)
    {
        var config = unitOfWork.Context.Set<TConfiguracionDelivery>()
            .FirstOrDefault(x => x.Activo);

        if (config != null)
            return config;

        config = new TConfiguracionDelivery
        {
            CostoBase = DefaultBaseCost,
            CostoPorKilometro = DefaultCostPerKilometer,
            LatitudCentro = DefaultLatitude,
            LongitudCentro = DefaultLongitude,
            RadioMaximoKm = DefaultRadiusKm,
            Activo = true,
            UsuarioCreacion = "sistema",
            FechaCreacion = DateTime.UtcNow
        };

        unitOfWork.Context.Set<TConfiguracionDelivery>().Add(config);
        unitOfWork.Commit();
        return config;
    }

    public static DeliveryConfigurationDTO ToDto(TConfiguracionDelivery config) => new()
    {
        CostoBase = config.CostoBase,
        CostoPorKilometro = config.CostoPorKilometro,
        LatitudCentro = config.LatitudCentro,
        LongitudCentro = config.LongitudCentro,
        RadioMaximoKm = config.RadioMaximoKm,
        Activo = config.Activo
    };

    public static decimal DistanceInKilometers(
        decimal latitude,
        decimal longitude,
        TConfiguracionDelivery config)
    {
        const double earthRadiusKm = 6372.8;
        var latitude1 = DegreesToRadians((double)config.LatitudCentro);
        var latitude2 = DegreesToRadians((double)latitude);
        var deltaLatitude = DegreesToRadians((double)(latitude - config.LatitudCentro));
        var deltaLongitude = DegreesToRadians((double)(longitude - config.LongitudCentro));

        var a = Math.Sin(deltaLatitude / 2) * Math.Sin(deltaLatitude / 2) +
                Math.Cos(latitude1) * Math.Cos(latitude2) *
                Math.Sin(deltaLongitude / 2) * Math.Sin(deltaLongitude / 2);
        var distance = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return (decimal)(earthRadiusKm * distance);
    }

    public static decimal CalculateCost(
        TConfiguracionDelivery config,
        decimal? latitude,
        decimal? longitude)
    {
        if (latitude.HasValue != longitude.HasValue)
            throw new InvalidOperationException("La ubicación debe incluir latitud y longitud.");

        if (!latitude.HasValue || !longitude.HasValue)
            return decimal.Round(config.CostoBase, 2);

        if (latitude is < -90 or > 90 || longitude is < -180 or > 180)
            throw new InvalidOperationException("Las coordenadas de delivery no son válidas.");

        var distance = DistanceInKilometers(latitude.Value, longitude.Value, config);
        if (distance > config.RadioMaximoKm)
            throw new InvalidOperationException("La ubicación está fuera del área de delivery de Cusco.");

        // 0-0.99 km uses only the base; each completed kilometer adds one rate.
        var additionalKilometers = decimal.Floor(distance);
        return decimal.Round(
            config.CostoBase + additionalKilometers * config.CostoPorKilometro, 2);
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180d;
}
