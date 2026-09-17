using TemplateProject.Dominio.Comun;

namespace TemplateProject.Dominio.Entidades
{
    public class Ingrediente : EntidadAuditable
    {
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
            string usuarioCreacionId,
            TimeProvider tiempo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ExcepcionDominio("El nombre es obligatorio.");

            if (precioUnitario < 0)
                throw new ExcepcionDominio("El precio unitario no puede ser negativo.");

            if (stockMinimo < 0)
                throw new ExcepcionDominio("El stock mínimo no puede ser negativo.");

            if (categoriaIngredienteId == Guid.Empty)
                throw new ExcepcionDominio("La categoría del ingrediente es obligatoria.");

            if (unidadMedidaId == Guid.Empty)
                throw new ExcepcionDominio("La unidad de medida es obligatoria.");

            var ingrediente = new Ingrediente
            {
                Id = Guid.NewGuid(),
                Nombre = nombre.Trim(),
                Descripcion = descripcion,
                Activo = true,
                PrecioUnitario = precioUnitario,
                StockActual = 0,
                StockMinimo = stockMinimo,
                CategoriaIngredienteId = categoriaIngredienteId,
                UnidadMedidaId = unidadMedidaId
            };

            ingrediente.RegistrarCreacion(usuarioCreacionId, tiempo);
            return ingrediente;
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

        public void ActualizarStockMinimo(decimal stockMinimo, TimeProvider tiempo)
        {
            if (stockMinimo < 0)
                throw new ExcepcionDominio("El stock mínimo no puede ser negativo.");

            StockMinimo = stockMinimo;
            RegistrarModificacion(tiempo);
        }

        public void CambiarCategoria(Guid categoriaIngredienteId, TimeProvider tiempo)
        {
            if (categoriaIngredienteId == Guid.Empty)
                throw new ExcepcionDominio("La categoría del ingrediente es obligatoria.");

            CategoriaIngredienteId = categoriaIngredienteId;
            RegistrarModificacion(tiempo);
        }

        public void CambiarUnidadMedida(Guid unidadMedidaId, TimeProvider tiempo)
        {
            if (unidadMedidaId == Guid.Empty)
                throw new ExcepcionDominio("La unidad de medida es obligatoria.");

            UnidadMedidaId = unidadMedidaId;
            RegistrarModificacion(tiempo);
        }

        public void AjustarStock(decimal cantidad, TimeProvider tiempo)
        {
            var nuevoStock = StockActual + cantidad;
            if (nuevoStock < 0)
                throw new ExcepcionDominio("El ajuste dejaría el stock en negativo.");

            StockActual = nuevoStock;
            RegistrarModificacion(tiempo);
        }

        public void ActualizarPrecio(decimal nuevoPrecio, TimeProvider tiempo)
        {
            if (nuevoPrecio < 0)
                throw new ExcepcionDominio("El precio unitario no puede ser negativo.");

            PrecioUnitario = nuevoPrecio;
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

        public bool StockPorDebajoDelMinimo() => StockActual <= StockMinimo;
    }
}
