namespace TemplateProject.Dominio.Entidades
{
    public class Ingrediente
    {
        public Guid Id { get; private set; }
        public string Nombre { get; private set; } = null!;
        public string? Descripcion { get; private set; }
        public bool Activo { get; private set; } = true;

        public decimal PrecioUnitario { get; private set; }
        public decimal StockActual { get; private set; }
        public decimal StockMinimo { get; private set; }

        public Guid CategoriaIngredienteId { get; private set; }
        public CategoriaIngrediente? CategoriaIngrediente { get; private set; }
        public Guid UnidadMedidaId { get; private set; }
        public UnidadMedida? UnidadMedida { get; private set; }

        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaModificacion { get; private set; }
        public string UsuarioCreacionId { get; private set; } = null!;

        private Ingrediente()
        {
        }

        public static Ingrediente Crear(
            string nombre,
            string? descripcion,
            decimal precioUnitario,
            decimal stockMinimo,
            Guid categoriaIngredienteId,
            Guid unidadMedidaId,
            string usuarioCreacionId)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre es obligatorio.", nameof(nombre));

            if (precioUnitario < 0)
                throw new ArgumentException("El precio unitario no puede ser negativo.", nameof(precioUnitario));

            if (stockMinimo < 0)
                throw new ArgumentException("El stock mínimo no puede ser negativo.", nameof(stockMinimo));

            if (string.IsNullOrWhiteSpace(usuarioCreacionId))
                throw new ArgumentException("El usuario de creación es obligatorio.", nameof(usuarioCreacionId));

            return new Ingrediente
            {
                Id = Guid.NewGuid(),
                Nombre = nombre.Trim(),
                Descripcion = descripcion,
                Activo = true,
                PrecioUnitario = precioUnitario,
                StockActual = 0,
                StockMinimo = stockMinimo,
                CategoriaIngredienteId = categoriaIngredienteId,
                UnidadMedidaId = unidadMedidaId,
                UsuarioCreacionId = usuarioCreacionId,
                FechaCreacion = DateTime.UtcNow
            };
        }

        public void AjustarStock(decimal cantidad)
        {
            var nuevoStock = StockActual + cantidad;
            if (nuevoStock < 0)
                throw new InvalidOperationException("El ajuste dejaría el stock en negativo.");

            StockActual = nuevoStock;
            FechaModificacion = DateTime.UtcNow;
        }

        public void ActualizarPrecio(decimal nuevoPrecio)
        {
            if (nuevoPrecio < 0)
                throw new ArgumentException("El precio unitario no puede ser negativo.", nameof(nuevoPrecio));

            PrecioUnitario = nuevoPrecio;
            FechaModificacion = DateTime.UtcNow;
        }

        public void Activar()
        {
            Activo = true;
            FechaModificacion = DateTime.UtcNow;
        }

        public void Desactivar()
        {
            Activo = false;
            FechaModificacion = DateTime.UtcNow;
        }

        public bool StockPorDebajoDelMinimo() => StockActual < StockMinimo;
    }
}
