namespace TemplateProject.Dominio.Entidades
{
    public class UnidadMedida
    {
        private readonly List<Ingrediente> _ingredientes = new();

        public Guid Id { get; private set; }
        public string Nombre { get; private set; } = null!;
        public string Abreviatura { get; private set; } = null!;
        public bool Activo { get; private set; } = true;

        public IReadOnlyCollection<Ingrediente> Ingredientes => _ingredientes.AsReadOnly();

        private UnidadMedida()
        {
        }

        public static UnidadMedida Crear(string nombre, string abreviatura)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            if (string.IsNullOrWhiteSpace(abreviatura))
                throw new ArgumentException("La abreviatura es obligatoria.", nameof(abreviatura));

            return new UnidadMedida
            {
                Id = Guid.NewGuid(),
                Nombre = nombre.Trim(),
                Abreviatura = abreviatura.Trim(),
                Activo = true
            };
        }

        public void ActualizarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            Nombre = nombre.Trim();
        }

        public void ActualizarAbreviatura(string abreviatura)
        {
            if (string.IsNullOrWhiteSpace(abreviatura))
                throw new ArgumentException("La abreviatura es obligatoria.", nameof(abreviatura));

            Abreviatura = abreviatura.Trim();
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
