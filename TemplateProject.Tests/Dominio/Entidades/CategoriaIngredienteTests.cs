using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Entidades
{
    public class CategoriaIngredienteTests
    {
        [Fact]
        public void Crear_DatosValidos_InicializaActivaYSinIngredientes()
        {
            var categoria = CategoriaIngrediente.Crear("Lácteos", "Productos lácteos");

            Assert.Equal("Lácteos", categoria.Nombre);
            Assert.Equal("Productos lácteos", categoria.Descripcion);
            Assert.True(categoria.Activo);
            Assert.Empty(categoria.Ingredientes);
            Assert.NotEqual(Guid.Empty, categoria.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_NombreInvalido_LanzaExcepcion(string? nombre)
        {
            Assert.Throws<ArgumentException>(() => CategoriaIngrediente.Crear(nombre!, "desc"));
        }

        [Fact]
        public void ActualizarNombre_ValorValido_ActualizaYRecortaEspacios()
        {
            var categoria = CategoriaIngrediente.Crear("Lácteos", null);

            categoria.ActualizarNombre("  Carnes  ");

            Assert.Equal("Carnes", categoria.Nombre);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ActualizarNombre_ValorInvalido_LanzaExcepcion(string nombre)
        {
            var categoria = CategoriaIngrediente.Crear("Lácteos", null);

            Assert.Throws<ArgumentException>(() => categoria.ActualizarNombre(nombre));
        }

        [Fact]
        public void ActualizarDescripcion_PermiteNulo()
        {
            var categoria = CategoriaIngrediente.Crear("Lácteos", "Descripción inicial");

            categoria.ActualizarDescripcion(null);

            Assert.Null(categoria.Descripcion);
        }

        [Fact]
        public void Desactivar_CategoriaActiva_QuedaInactiva()
        {
            var categoria = CategoriaIngrediente.Crear("Lácteos", null);

            categoria.Desactivar();

            Assert.False(categoria.Activo);
        }

        [Fact]
        public void Activar_CategoriaInactiva_QuedaActiva()
        {
            var categoria = CategoriaIngrediente.Crear("Lácteos", null);
            categoria.Desactivar();

            categoria.Activar();

            Assert.True(categoria.Activo);
        }
    }
}
