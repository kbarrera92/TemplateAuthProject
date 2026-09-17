using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.DTOs.CategoriasIngredientes;

namespace TemplateProject.Controllers
{
    [ApiController]
    [Route("api/categorias-ingredientes")]
    [Authorize]
    public class CategoriasIngredientesController : ControllerBase
    {
        private readonly IServicioCategoriasIngredientes servicio;

        public CategoriasIngredientesController(IServicioCategoriasIngredientes servicio)
        {
            this.servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaIngredienteDTO>>> Listar([FromQuery] bool? activo)
        {
            return await servicio.Listar(activo);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CategoriaIngredienteDTO>> ObtenerPorId(Guid id)
        {
            var categoria = await servicio.ObtenerPorId(id);
            return categoria is null ? NotFound() : categoria;
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaIngredienteDTO>> Crear(CrearCategoriaIngredienteDTO dto)
        {
            var categoria = await servicio.Crear(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = categoria.Id }, categoria);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CategoriaIngredienteDTO>> Actualizar(Guid id, ActualizarCategoriaIngredienteDTO dto)
        {
            var categoria = await servicio.Actualizar(id, dto);
            return categoria is null ? NotFound() : categoria;
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Desactivar(Guid id)
        {
            var desactivado = await servicio.Desactivar(id);
            return desactivado ? NoContent() : NotFound();
        }
    }
}
