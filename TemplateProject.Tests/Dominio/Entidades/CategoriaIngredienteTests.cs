using Microsoft.Extensions.Time.Testing;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Entidades
{
    public class CategoriaIngredienteTests
    {
        private static readonly DateTimeOffset InstanteInicial = new(2026, 9, 17, 6, 0, 0, TimeSpan.Zero);

        private static FakeTimeProvider CrearReloj() => new(InstanteInicial);

        private static CategoriaIngrediente CrearCategoriaValida(FakeTimeProvider tiempo)
        {
            return CategoriaIngrediente.Crear("Lácteos", "Productos lácteos", "usuario-1", tiempo);
        }

        [Fact]
        public void Crear_DatosValidos_InicializaActivaYSinIngredientes()
        {
            var tiempo = CrearReloj();

            var categoria = CrearCategoriaValida(tiempo);

            Assert.Equal("Lácteos", categoria.Nombre);
            Assert.Equal("Productos lácteos", categoria.Descripcion);
            Assert.True(categoria.Activo);
            Assert.Empty(categoria.Ingredientes);
            Assert.NotEqual(Guid.Empty, categoria.Id);
            Assert.Equal("usuario-1", categoria.UsuarioCreacionId);
            Assert.Equal(InstanteInicial.UtcDateTime, categoria.FechaCreacion);
            Assert.Null(categoria.FechaModificacion);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_NombreInvalido_LanzaExcepcion(string? nombre)
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => CategoriaIngrediente.Crear(nombre!, "desc", "usuario-1", tiempo));
        }

        [Fact]
        public void Crear_UsuarioCreacionVacio_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => CategoriaIngrediente.Crear("Lácteos", "desc", " ", tiempo));
        }

        [Fact]
        public void ActualizarNombre_ValorValido_ActualizaYRecortaEspacios()
        {
            var tiempo = CrearReloj();
            var categoria = CrearCategoriaValida(tiempo);

            categoria.ActualizarNombre("  Carnes  ", tiempo);

            Assert.Equal("Carnes", categoria.Nombre);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ActualizarNombre_ValorInvalido_LanzaExcepcion(string nombre)
        {
            var tiempo = CrearReloj();
            var categoria = CrearCategoriaValida(tiempo);

            Assert.Throws<ExcepcionDominio>(() => categoria.ActualizarNombre(nombre, tiempo));
        }

        [Fact]
        public void ActualizarNombre_RegistraFechaModificacion()
        {
            var tiempo = CrearReloj();
            var categoria = CrearCategoriaValida(tiempo);
            var instanteCambio = InstanteInicial.AddHours(3);
            tiempo.SetUtcNow(instanteCambio);

            categoria.ActualizarNombre("Carnes", tiempo);

            Assert.Equal(instanteCambio.UtcDateTime, categoria.FechaModificacion);
        }

        [Fact]
        public void ActualizarDescripcion_PermiteNulo()
        {
            var tiempo = CrearReloj();
            var categoria = CrearCategoriaValida(tiempo);

            categoria.ActualizarDescripcion(null, tiempo);

            Assert.Null(categoria.Descripcion);
        }

        [Fact]
        public void Desactivar_CategoriaActiva_QuedaInactiva()
        {
            var tiempo = CrearReloj();
            var categoria = CrearCategoriaValida(tiempo);

            categoria.Desactivar(tiempo);

            Assert.False(categoria.Activo);
        }

        [Fact]
        public void Activar_CategoriaInactiva_QuedaActiva()
        {
            var tiempo = CrearReloj();
            var categoria = CrearCategoriaValida(tiempo);
            categoria.Desactivar(tiempo);

            categoria.Activar(tiempo);

            Assert.True(categoria.Activo);
        }
    }
}
