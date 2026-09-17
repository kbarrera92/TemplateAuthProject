using Microsoft.Extensions.Time.Testing;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Entidades
{
    public class UnidadMedidaTests
    {
        private static readonly DateTimeOffset InstanteInicial = new(2026, 9, 17, 6, 0, 0, TimeSpan.Zero);

        private static FakeTimeProvider CrearReloj() => new(InstanteInicial);

        private static UnidadMedida CrearUnidadValida(FakeTimeProvider tiempo)
        {
            return UnidadMedida.Crear("Kilogramo", "kg", "usuario-1", tiempo);
        }

        [Fact]
        public void Crear_DatosValidos_InicializaActivaYSinIngredientes()
        {
            var tiempo = CrearReloj();

            var unidad = CrearUnidadValida(tiempo);

            Assert.Equal("Kilogramo", unidad.Nombre);
            Assert.Equal("kg", unidad.Abreviatura);
            Assert.True(unidad.Activo);
            Assert.Empty(unidad.Ingredientes);
            Assert.NotEqual(Guid.Empty, unidad.Id);
            Assert.Equal("usuario-1", unidad.UsuarioCreacionId);
            Assert.Equal(InstanteInicial.UtcDateTime, unidad.FechaCreacion);
            Assert.Null(unidad.FechaModificacion);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_NombreInvalido_LanzaExcepcion(string? nombre)
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => UnidadMedida.Crear(nombre!, "kg", "usuario-1", tiempo));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_AbreviaturaInvalida_LanzaExcepcion(string? abreviatura)
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => UnidadMedida.Crear("Kilogramo", abreviatura!, "usuario-1", tiempo));
        }

        [Fact]
        public void Crear_UsuarioCreacionVacio_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => UnidadMedida.Crear("Kilogramo", "kg", " ", tiempo));
        }

        [Fact]
        public void ActualizarNombre_ValorValido_ActualizaYRecortaEspacios()
        {
            var tiempo = CrearReloj();
            var unidad = CrearUnidadValida(tiempo);

            unidad.ActualizarNombre("  Litro  ", tiempo);

            Assert.Equal("Litro", unidad.Nombre);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ActualizarNombre_ValorInvalido_LanzaExcepcion(string nombre)
        {
            var tiempo = CrearReloj();
            var unidad = CrearUnidadValida(tiempo);

            Assert.Throws<ExcepcionDominio>(() => unidad.ActualizarNombre(nombre, tiempo));
        }

        [Fact]
        public void ActualizarAbreviatura_ValorValido_ActualizaYRecortaEspacios()
        {
            var tiempo = CrearReloj();
            var unidad = CrearUnidadValida(tiempo);

            unidad.ActualizarAbreviatura("  Kg  ", tiempo);

            Assert.Equal("Kg", unidad.Abreviatura);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ActualizarAbreviatura_ValorInvalido_LanzaExcepcion(string abreviatura)
        {
            var tiempo = CrearReloj();
            var unidad = CrearUnidadValida(tiempo);

            Assert.Throws<ExcepcionDominio>(() => unidad.ActualizarAbreviatura(abreviatura, tiempo));
        }

        [Fact]
        public void ActualizarAbreviatura_RegistraFechaModificacion()
        {
            var tiempo = CrearReloj();
            var unidad = CrearUnidadValida(tiempo);
            var instanteCambio = InstanteInicial.AddHours(3);
            tiempo.SetUtcNow(instanteCambio);

            unidad.ActualizarAbreviatura("Kg", tiempo);

            Assert.Equal(instanteCambio.UtcDateTime, unidad.FechaModificacion);
        }

        [Fact]
        public void Desactivar_UnidadActiva_QuedaInactiva()
        {
            var tiempo = CrearReloj();
            var unidad = CrearUnidadValida(tiempo);

            unidad.Desactivar(tiempo);

            Assert.False(unidad.Activo);
        }

        [Fact]
        public void Activar_UnidadInactiva_QuedaActiva()
        {
            var tiempo = CrearReloj();
            var unidad = CrearUnidadValida(tiempo);
            unidad.Desactivar(tiempo);

            unidad.Activar(tiempo);

            Assert.True(unidad.Activo);
        }
    }
}
