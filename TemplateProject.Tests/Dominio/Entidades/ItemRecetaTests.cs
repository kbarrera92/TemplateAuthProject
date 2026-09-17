using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Entidades
{
    public class ItemRecetaTests
    {
        [Fact]
        public void Crear_DatosValidos_InicializaCorrectamente()
        {
            var platoId = Guid.NewGuid();
            var ingredienteId = Guid.NewGuid();

            var item = ItemReceta.Crear(platoId, ingredienteId, 2.5m);

            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal(platoId, item.PlatoId);
            Assert.Equal(ingredienteId, item.IngredienteId);
            Assert.Equal(2.5m, item.Cantidad);
        }

        [Fact]
        public void Crear_PlatoVacio_LanzaExcepcion()
        {
            Assert.Throws<ExcepcionDominio>(() => ItemReceta.Crear(Guid.Empty, Guid.NewGuid(), 1m));
        }

        [Fact]
        public void Crear_IngredienteVacio_LanzaExcepcion()
        {
            Assert.Throws<ExcepcionDominio>(() => ItemReceta.Crear(Guid.NewGuid(), Guid.Empty, 1m));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Crear_CantidadInvalida_LanzaExcepcion(decimal cantidad)
        {
            Assert.Throws<ExcepcionDominio>(() => ItemReceta.Crear(Guid.NewGuid(), Guid.NewGuid(), cantidad));
        }

        [Fact]
        public void ActualizarCantidad_ValorValido_Actualiza()
        {
            var item = ItemReceta.Crear(Guid.NewGuid(), Guid.NewGuid(), 1m);

            item.ActualizarCantidad(3m);

            Assert.Equal(3m, item.Cantidad);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-2)]
        public void ActualizarCantidad_ValorInvalido_LanzaExcepcion(decimal cantidad)
        {
            var item = ItemReceta.Crear(Guid.NewGuid(), Guid.NewGuid(), 1m);

            Assert.Throws<ExcepcionDominio>(() => item.ActualizarCantidad(cantidad));
        }
    }
}
