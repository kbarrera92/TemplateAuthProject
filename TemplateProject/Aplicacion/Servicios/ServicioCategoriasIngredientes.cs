using Microsoft.EntityFrameworkCore;
using TemplateProject.Datos;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using TemplateProject.DTOs.CategoriasIngredientes;

namespace TemplateProject.Aplicacion.Servicios
{
    public class ServicioCategoriasIngredientes : IServicioCategoriasIngredientes
    {
        private readonly ApplicationDbContext contexto;
        private readonly TimeProvider tiempo;
        private readonly IProveedorUsuario proveedorUsuario;

        public ServicioCategoriasIngredientes(ApplicationDbContext contexto, TimeProvider tiempo, IProveedorUsuario proveedorUsuario)
        {
            this.contexto = contexto;
            this.tiempo = tiempo;
            this.proveedorUsuario = proveedorUsuario;
        }

        public async Task<List<CategoriaIngredienteDTO>> Listar(bool? activo)
        {
            var consulta = contexto.CategoriasIngredientes.AsQueryable();

            if (activo is not null)
                consulta = consulta.Where(x => x.Activo == activo);

            var categorias = await consulta.OrderBy(x => x.Nombre).ToListAsync();
            return categorias.Select(MapearADTO).ToList();
        }

        public async Task<CategoriaIngredienteDTO?> ObtenerPorId(Guid id)
        {
            var categoria = await contexto.CategoriasIngredientes.FindAsync(id);
            return categoria is null ? null : MapearADTO(categoria);
        }

        public async Task<CategoriaIngredienteDTO> Crear(CrearCategoriaIngredienteDTO dto)
        {
            await ValidarNombreDisponible(dto.Nombre, idAExcluir: null);

            var usuarioId = proveedorUsuario.ObtenerUsuarioId();
            var categoria = CategoriaIngrediente.Crear(dto.Nombre, dto.Descripcion, usuarioId, tiempo);

            contexto.CategoriasIngredientes.Add(categoria);
            await contexto.SaveChangesAsync();

            return MapearADTO(categoria);
        }

        public async Task<CategoriaIngredienteDTO?> Actualizar(Guid id, ActualizarCategoriaIngredienteDTO dto)
        {
            var categoria = await contexto.CategoriasIngredientes.FindAsync(id);
            if (categoria is null)
                return null;

            await ValidarNombreDisponible(dto.Nombre, idAExcluir: id);

            categoria.ActualizarNombre(dto.Nombre, tiempo);
            categoria.ActualizarDescripcion(dto.Descripcion, tiempo);

            await contexto.SaveChangesAsync();
            return MapearADTO(categoria);
        }

        public async Task<bool> Desactivar(Guid id)
        {
            var categoria = await contexto.CategoriasIngredientes.FindAsync(id);
            if (categoria is null)
                return false;

            categoria.Desactivar(tiempo);
            await contexto.SaveChangesAsync();
            return true;
        }

        private async Task ValidarNombreDisponible(string nombre, Guid? idAExcluir)
        {
            var nombreNormalizado = nombre.Trim();
            var existe = await contexto.CategoriasIngredientes
                .AnyAsync(x => x.Nombre == nombreNormalizado && x.Id != (idAExcluir ?? Guid.Empty));

            if (existe)
                throw new ExcepcionDominio($"Ya existe una categoría de ingrediente con el nombre '{nombreNormalizado}'.");
        }

        private static CategoriaIngredienteDTO MapearADTO(CategoriaIngrediente categoria) => new()
        {
            Id = categoria.Id,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            Activo = categoria.Activo,
            FechaCreacion = categoria.FechaCreacion,
            FechaModificacion = categoria.FechaModificacion,
            UsuarioCreacionId = categoria.UsuarioCreacionId
        };
    }
}
