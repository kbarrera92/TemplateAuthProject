using TemplateProject.Dominio.Comun;

namespace TemplateProject.Dominio.Entidades
{
    public class UnidadMedida : EntidadAuditable
    {
        private readonly List<Ingrediente> _ingredientes = new();

        public string Nombre { get; private set; } = null!;
        public string Abreviatura { get; private set; } = null!;
        public bool Activo { get; private set; } = true;

        public IReadOnlyCollection<Ingrediente> Ingredientes => _ingredientes.AsReadOnly();

        private UnidadMedida()
        {
        }

        public static UnidadMedida Crear(string nombre, string abreviatura, string usuarioCreacionId, TimeProvider tiempo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ExcepcionDominio("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(abreviatura))
                throw new ExcepcionDominio("La abreviatura es obligatoria.");

            var unidad = new UnidadMedida
            {
                Id = Guid.NewGuid(),
                Nombre = nombre.Trim(),
                Abreviatura = abreviatura.Trim(),
                Activo = true
            };

            unidad.RegistrarCreacion(usuarioCreacionId, tiempo);
            return unidad;
        }

        public void ActualizarNombre(string nombre, TimeProvider tiempo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ExcepcionDominio("El nombre es obligatorio.");

            Nombre = nombre.Trim();
            RegistrarModificacion(tiempo);
        }

        public void ActualizarAbreviatura(string abreviatura, TimeProvider tiempo)
        {
            if (string.IsNullOrWhiteSpace(abreviatura))
                throw new ExcepcionDominio("La abreviatura es obligatoria.");

            Abreviatura = abreviatura.Trim();
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
