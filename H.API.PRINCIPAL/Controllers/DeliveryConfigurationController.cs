using H.DTOs;
using H.Services;
using H.DataAccess.UnitofWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace H.API.PRINCIPAL.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DeliveryConfigurationController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public DeliveryConfigurationController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok(DeliveryPricingService.ToDto(
                DeliveryPricingService.GetOrCreate(_unitOfWork)));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    [Authorize(Roles = "Administrador")]
    public IActionResult Update([FromBody] DeliveryConfigurationDTO dto)
    {
        if (dto == null || dto.CostoBase <= 0 || dto.CostoPorKilometro < 0 ||
            dto.RadioMaximoKm <= 0 || dto.LatitudCentro is < -90 or > 90 ||
            dto.LongitudCentro is < -180 or > 180)
            return BadRequest(new { message = "Los valores de configuración de delivery no son válidos." });

        try
        {
            var config = DeliveryPricingService.GetOrCreate(_unitOfWork);
            config.CostoBase = decimal.Round(dto.CostoBase, 2);
            config.CostoPorKilometro = decimal.Round(dto.CostoPorKilometro, 2);
            config.LatitudCentro = dto.LatitudCentro;
            config.LongitudCentro = dto.LongitudCentro;
            config.RadioMaximoKm = dto.RadioMaximoKm;
            config.Activo = true;
            config.UsuarioModificacion = User.Identity?.Name ?? "admin";
            config.FechaModificacion = DateTime.UtcNow;
            _unitOfWork.Context.Set<H.DataAccess.Models.TConfiguracionDelivery>().Update(config);
            _unitOfWork.Commit();
            return Ok(DeliveryPricingService.ToDto(config));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
