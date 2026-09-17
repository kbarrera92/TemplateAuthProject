using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Entidades
{
    public class UnidadMedidaTests
    {
        [Fact]
        public void Crear_DatosValidos_InicializaActivaYSinIngredientes()
        {
            var unidad = UnidadMedida.Crear("Kilogramo", "kg");

            Assert.Equal("Kilogramo", unidad.Nombre);
            Assert.Equal("kg", unidad.Abreviatura);
            Assert.True(unidad.Activo);
            Assert.Empty(unidad.Ingredientes);
            Assert.NotEqual(Guid.Empty, unidad.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_NombreInvalido_LanzaExcepcion(string? nombre)
        {
            Assert.Throws<ArgumentException>(() => UnidadMedida.Crear(nombre!, "kg"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_AbreviaturaInvalida_LanzaExcepcion(string? abreviatura)
        {
            Assert.Throws<ArgumentException>(() => UnidadMedida.Crear("Kilogramo", abreviatura!));
        }

        [Fact]
        public void ActualizarNombre_ValorValido_ActualizaYRecortaEspacios()
        {
            var unidad = UnidadMedida.Crear("Kilogramo", "kg");

            unidad.ActualizarNombre("  Litro  ");

            Assert.Equal("Litro", unidad.Nombre);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ActualizarNombre_ValorInvalido_LanzaExcepcion(string nombre)
        {
            var unidad = UnidadMedida.Crear("Kilogramo", "kg");

            Assert.Throws<ArgumentException>(() => unidad.ActualizarNombre(nombre));
        }

        [Fact]
        public void ActualizarAbreviatura_ValorValido_ActualizaYRecortaEspacios()
        {
            var unidad = UnidadMedida.Crear("Kilogramo", "kg");

            unidad.ActualizarAbreviatura("  Kg  ");

            Assert.Equal("Kg", unidad.Abreviatura);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ActualizarAbreviatura_ValorInvalido_LanzaExcepcion(string abreviatura)
        {
            var unidad = UnidadMedida.Crear("Kilogramo", "kg");

            Assert.Throws<ArgumentException>(() => unidad.ActualizarAbreviatura(abreviatura));
        }

        [Fact]
        public void Desactivar_UnidadActiva_QuedaInactiva()
        {
            var unidad = UnidadMedida.Crear("Kilogramo", "kg");

            unidad.Desactivar();

            Assert.False(unidad.Activo);
        }

        [Fact]
        public void Activar_UnidadInactiva_QuedaActiva()
        {
            var unidad = UnidadMedida.Crear("Kilogramo", "kg");
            unidad.Desactivar();

            unidad.Activar();

            Assert.True(unidad.Activo);
        }
    }
}
