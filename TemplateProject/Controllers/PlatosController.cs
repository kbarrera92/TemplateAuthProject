using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.DTOs.Platos;

namespace TemplateProject.Controllers
{
    [ApiController]
    [Route("api/platos")]
    [Authorize]
    public class PlatosController : ControllerBase
    {
        private readonly IServicioPlatos servicio;

        public PlatosController(IServicioPlatos servicio)
        {
            this.servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<PlatoDTO>>> Listar([FromQuery] bool? activo)
        {
            return await servicio.Listar(activo);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PlatoDTO>> ObtenerPorId(Guid id)
        {
            var plato = await servicio.ObtenerPorId(id);
            return plato is null ? NotFound() : plato;
        }

        [HttpPost]
        public async Task<ActionResult<PlatoDTO>> Crear(CrearPlatoDTO dto)
        {
            var plato = await servicio.Crear(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = plato.Id }, plato);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<PlatoDTO>> Actualizar(Guid id, ActualizarPlatoDTO dto)
        {
            var plato = await servicio.Actualizar(id, dto);
            return plato is null ? NotFound() : plato;
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            var desactivado = await servicio.Desactivar(id);
            return desactivado ? NoContent() : NotFound();
        }

        [HttpPost("{id:guid}/activar")]
        public async Task<IActionResult> Activar(Guid id)
        {
            var activado = await servicio.Activar(id);
            return activado ? NoContent() : NotFound();
        }

        [HttpPost("{id:guid}/receta")]
        public async Task<ActionResult<PlatoDTO>> AgregarIngrediente(Guid id, AgregarItemRecetaDTO dto)
        {
            var plato = await servicio.AgregarIngrediente(id, dto);
            return plato is null ? NotFound() : plato;
        }

        [HttpPut("{id:guid}/receta/{ingredienteId:guid}")]
        public async Task<ActionResult<PlatoDTO>> ActualizarCantidadIngrediente(Guid id, Guid ingredienteId, ActualizarItemRecetaDTO dto)
        {
            var plato = await servicio.ActualizarCantidadIngrediente(id, ingredienteId, dto);
            return plato is null ? NotFound() : plato;
        }

        [HttpDelete("{id:guid}/receta/{ingredienteId:guid}")]
        public async Task<ActionResult<PlatoDTO>> QuitarIngrediente(Guid id, Guid ingredienteId)
        {
            var plato = await servicio.QuitarIngrediente(id, ingredienteId);
            return plato is null ? NotFound() : plato;
        }
    }
}
