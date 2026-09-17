using TemplateProject.Dominio.Comun;

namespace TemplateProject.Dominio.Entidades
{
    public class CategoriaIngrediente : EntidadAuditable
    {
        private readonly List<Ingrediente> _ingredientes = new();

        public string Nombre { get; private set; } = null!;
        public string? Descripcion { get; private set; }
        public bool Activo { get; private set; } = true;

        public IReadOnlyCollection<Ingrediente> Ingredientes => _ingredientes.AsReadOnly();

        private CategoriaIngrediente()
        {
        }

        public static CategoriaIngrediente Crear(string nombre, string? descripcion, string usuarioCreacionId, TimeProvider tiempo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ExcepcionDominio("El nombre es obligatorio.");

            var categoria = new CategoriaIngrediente
            {
                Id = Guid.NewGuid(),
                Nombre = nombre.Trim(),
                Descripcion = descripcion,
                Activo = true
            };

            categoria.RegistrarCreacion(usuarioCreacionId, tiempo);
            return categoria;
        }

        public void ActualizarNombre(string nombre, TimeProvider tiempo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ExcepcionDominio("El nombre es obligatorio.");

            Nombre = nombre.Trim();
            RegistrarModificacion(tiempo);
        }

        public void ActualizarDescripcion(string? descripcion, TimeProvider tiempo)
        {
            Descripcion = descripcion;
            RegistrarModificacion(tiempo);
        }

        public void Activar(TimeProvider tiempo)
        {
            Activo = true;
            RegistrarModificacion(tiempo);
        }

        public void Desactivar(TimeProvider tiempo)
        {
            Activo = false;
            RegistrarModificacion(tiempo);
        }
    }
}
