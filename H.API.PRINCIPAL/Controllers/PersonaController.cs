using H.DataAccess.Entidades;
using H.DataAccess.Helpers;
using H.Services;
using H.DataAccess.Extension;
using H.DataAccess.UnitofWork;
using H.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace H.API.PRINCIPAL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonaController : ControllerBase
    {
        private IUnitOfWork unitOfWork;

        public PersonaController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpPost("Insertar")]
        public IActionResult Insert([FromBody] Persona categoria)
        {
            try
            {
                var servicio = new PersonaService(unitOfWork);
                categoria.FechaCreacion = Fecha.Hoy;
                categoria.FechaModificacion = Fecha.Hoy;
                var respuesta = servicio.Add(categoria);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Modificar")]
        public IActionResult Update([FromBody] Persona categoria)
        {
            try
            {
                var servicio = new PersonaService(unitOfWork);
                categoria.FechaModificacion = Fecha.Hoy;
                var respuesta = servicio.Update(categoria);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpDelete("Eliminar")]
        public IActionResult Delete(int id, string usuario)
        {
            try
            {
                var servicio = new PersonaService(unitOfWork);
                var respuesta = servicio.Delete(id, usuario);
                return Ok(respuesta);
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerListadoPorId")]
        public IActionResult ObtenerListadoPorId(int id)
        {
            try
            {
                var servicio = new PersonaService(unitOfWork);
                return Ok(servicio.GetById(id));
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpGet("ObtenerCombo")]
        public IActionResult ObtenerCombo()
        {
            try
            {
                var servicio = new PersonaService(unitOfWork);
                return Ok(servicio.ObtenerCombo());
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }

        [HttpPut("Actualizar")]
        public IActionResult Actualizar([FromBody] ActualizarPersonaDTO dto)
        {
            try
            {
                var persona = unitOfWork.PersonaRepository.GetById(dto.Id);
                if (persona == null)
                    return NotFound(new { mensaje = "Persona no encontrada" });

                persona.IdTipoDocumento = dto.IdTipoDocumento;
                persona.NumeroDocumento = dto.NumeroDocumento;
                persona.Nombres = dto.Nombres;
                persona.ApellidoPaterno = dto.ApellidoPaterno;
                persona.ApellidoMaterno = dto.ApellidoMaterno;
                persona.Telefono = dto.Telefono;
                persona.Direccion = dto.Direccion;
                persona.FechaModificacion = H.DataAccess.Helpers.Fecha.Hoy;

                unitOfWork.PersonaRepository.Update(persona);
                unitOfWork.Commit();

                return Ok(new { success = true, mensaje = "Datos actualizados correctamente" });
            }
            catch (Exception ex)
            {
                return new ErrorResult(ex, User);
            }
        }
    }
}