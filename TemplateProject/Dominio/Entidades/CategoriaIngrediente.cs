namespace TemplateProject.Dominio.Entidades
{
    public class CategoriaIngrediente
    {
        private readonly List<Ingrediente> _ingredientes = new();

        public Guid Id { get; private set; }
        public string Nombre { get; private set; } = null!;
        public string? Descripcion { get; private set; }
        public bool Activo { get; private set; } = true;

        public IReadOnlyCollection<Ingrediente> Ingredientes => _ingredientes.AsReadOnly();

        private CategoriaIngrediente()
        {
        }

        public static CategoriaIngrediente Crear(string nombre, string? descripcion)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            return new CategoriaIngrediente
            {
                Id = Guid.NewGuid(),
                Nombre = nombre.Trim(),
                Descripcion = descripcion,
                Activo = true
            };
        }

        public void ActualizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            Nombre = nombre.Trim();
        }

        public void ActualizarDescripcion(string? descripcion)
        {
            Descripcion = descripcion;
        }

        public void Activar()
        {
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }
    }
}
