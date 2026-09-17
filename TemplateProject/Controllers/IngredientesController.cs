using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.DTOs.Ingredientes;

namespace TemplateProject.Controllers
{
    [ApiController]
    [Route("api/ingredientes")]
    [Authorize]
    public class IngredientesController : ControllerBase
    {
        private readonly IServicioIngredientes servicio;

        public IngredientesController(IServicioIngredientes servicio)
        {
            this.servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<IngredienteDTO>>> Listar([FromQuery] bool? activo)
        {
            return await servicio.Listar(activo);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<IngredienteDTO>> ObtenerPorId(Guid id)
        {
            var ingrediente = await servicio.ObtenerPorId(id);
            return ingrediente is null ? NotFound() : ingrediente;
        }

        [HttpPost]
        public async Task<ActionResult<IngredienteDTO>> Crear(CrearIngredienteDTO dto)
        {
            var ingrediente = await servicio.Crear(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = ingrediente.Id }, ingrediente);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<IngredienteDTO>> Actualizar(Guid id, ActualizarIngredienteDTO dto)
        {
            var ingrediente = await servicio.Actualizar(id, dto);
            return ingrediente is null ? NotFound() : ingrediente;
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            var desactivado = await servicio.Desactivar(id);
            return desactivado ? NoContent() : NotFound();
        }

        [HttpPatch("{id:guid}/stock")]
        public async Task<ActionResult<IngredienteDTO>> AjustarStock(Guid id, AjustarStockDTO dto)
        {
            var ingrediente = await servicio.AjustarStock(id, dto.Cantidad);
            return ingrediente is null ? NotFound() : ingrediente;
        }
    }
}
