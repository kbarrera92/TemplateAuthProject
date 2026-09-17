using TemplateProject.Dominio.Comun;

namespace TemplateProject.Dominio.Entidades
{
    public class ItemReceta : EntidadBase
    {
        public Guid PlatoId { get; private set; }
        public Guid IngredienteId { get; private set; }
        public Ingrediente? Ingrediente { get; private set; }
        public decimal Cantidad { get; private set; }

        private ItemReceta()
        {
        }

        public static ItemReceta Crear(Guid platoId, Guid ingredienteId, decimal cantidad)
        {
            if (platoId == Guid.Empty)
                throw new ExcepcionDominio("El plato es obligatorio.");

            if (ingredienteId == Guid.Empty)
                throw new ExcepcionDominio("El ingrediente es obligatorio.");

            if (cantidad <= 0)
                throw new ExcepcionDominio("La cantidad debe ser mayor a cero.");

            return new ItemReceta
            {
                Id = Guid.NewGuid(),
                PlatoId = platoId,
                IngredienteId = ingredienteId,
                Cantidad = cantidad
            };
        }

        public void ActualizarCantidad(decimal cantidad)
        {
            if (cantidad <= 0)
                throw new ExcepcionDominio("La cantidad debe ser mayor a cero.");

            Cantidad = cantidad;
        }
    }
}
