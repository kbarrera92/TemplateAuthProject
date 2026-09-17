using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.DTOs.UnidadesMedida;

namespace TemplateProject.Controllers
{
    [ApiController]
    [Route("api/unidades-medida")]
    [Authorize]
    public class UnidadesMedidaController : ControllerBase
    {
        private readonly IServicioUnidadesMedida servicio;

        public UnidadesMedidaController(IServicioUnidadesMedida servicio)
        {
            this.servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<UnidadMedidaDTO>>> Listar([FromQuery] bool? activo)
        {
            return await servicio.Listar(activo);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<UnidadMedidaDTO>> ObtenerPorId(Guid id)
        {
            var unidad = await servicio.ObtenerPorId(id);
            return unidad is null ? NotFound() : unidad;
        }

        [HttpPost]
        public async Task<ActionResult<UnidadMedidaDTO>> Crear(CrearUnidadMedidaDTO dto)
        {
            var unidad = await servicio.Crear(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = unidad.Id }, unidad);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<UnidadMedidaDTO>> Actualizar(Guid id, ActualizarUnidadMedidaDTO dto)
        {
            var unidad = await servicio.Actualizar(id, dto);
            return unidad is null ? NotFound() : unidad;
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            var desactivado = await servicio.Desactivar(id);
            return desactivado ? NoContent() : NotFound();
        }
    }
}
