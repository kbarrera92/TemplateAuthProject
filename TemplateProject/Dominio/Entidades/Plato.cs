using TemplateProject.Dominio.Comun;

namespace TemplateProject.Dominio.Entidades
{
    public class Plato : EntidadAuditable
    {
        private readonly List<ItemReceta> _itemsReceta = new();

        public string Nombre { get; private set; } = null!;
        public string? Descripcion { get; private set; }
        public decimal PrecioVenta { get; private set; }
        public bool Activo { get; private set; } = true;

        public IReadOnlyCollection<ItemReceta> ItemsReceta => _itemsReceta.AsReadOnly();

        private Plato()
        {
        }

        public static Plato Crear(
            string nombre,
            string? descripcion,
            decimal precioVenta,
            string usuarioCreacionId,
            TimeProvider tiempo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ExcepcionDominio("El nombre es obligatorio.");

            if (precioVenta < 0)
                throw new ExcepcionDominio("El precio de venta no puede ser negativo.");

            var plato = new Plato
            {
                Id = Guid.NewGuid(),
                Nombre = nombre.Trim(),
                Descripcion = descripcion,
                PrecioVenta = precioVenta,
                Activo = true
            };

            plato.RegistrarCreacion(usuarioCreacionId, tiempo);
            return plato;
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

        public void ActualizarPrecioVenta(decimal precioVenta, TimeProvider tiempo)
        {
            if (precioVenta < 0)
                throw new ExcepcionDominio("El precio de venta no puede ser negativo.");

            PrecioVenta = precioVenta;
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

        public void AgregarIngrediente(Guid ingredienteId, decimal cantidad, TimeProvider tiempo)
        {
            if (_itemsReceta.Any(x => x.IngredienteId == ingredienteId))
                throw new ExcepcionDominio("El ingrediente ya forma parte de la receta.");

            _itemsReceta.Add(ItemReceta.Crear(Id, ingredienteId, cantidad));
            RegistrarModificacion(tiempo);
        }

        public void ActualizarCantidadIngrediente(Guid ingredienteId, decimal cantidad, TimeProvider tiempo)
        {
            var item = _itemsReceta.FirstOrDefault(x => x.IngredienteId == ingredienteId);
            if (item is null)
                throw new ExcepcionDominio("El ingrediente no forma parte de la receta.");

            item.ActualizarCantidad(cantidad);
            RegistrarModificacion(tiempo);
        }

        public void QuitarIngrediente(Guid ingredienteId, TimeProvider tiempo)
        {
            var item = _itemsReceta.FirstOrDefault(x => x.IngredienteId == ingredienteId);
            if (item is null)
                throw new ExcepcionDominio("El ingrediente no forma parte de la receta.");

            _itemsReceta.Remove(item);
            RegistrarModificacion(tiempo);
        }

        public bool TieneReceta() => _itemsReceta.Count > 0;
    }
}
