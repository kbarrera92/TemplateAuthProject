using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Entidades
{
    public class IngredienteTests
    {
        private static Ingrediente CrearIngredienteValido(decimal precioUnitario = 10m, decimal stockMinimo = 5m)
        {
            return Ingrediente.Crear(
                nombre: "Harina",
                descripcion: "Harina de trigo",
                precioUnitario: precioUnitario,
                stockMinimo: stockMinimo,
                categoriaIngredienteId: Guid.NewGuid(),
                unidadMedidaId: Guid.NewGuid(),
                usuarioCreacionId: "usuario-1");
        }

        [Fact]
        public void Crear_DatosValidos_InicializaConStockEnCeroYActivo()
        {
            var ingrediente = CrearIngredienteValido();

            Assert.Equal("Harina", ingrediente.Nombre);
            Assert.True(ingrediente.Activo);
            Assert.Equal(0m, ingrediente.StockActual);
            Assert.NotEqual(Guid.Empty, ingrediente.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_NombreInvalido_LanzaExcepcion(string? nombre)
        {
            Assert.Throws<ArgumentException>(() => Ingrediente.Crear(
                nombre!, "desc", 10m, 5m, Guid.NewGuid(), Guid.NewGuid(), "usuario-1"));
        }

        [Fact]
        public void Crear_PrecioNegativo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentException>(() => Ingrediente.Crear(
                "Harina", null, -1m, 5m, Guid.NewGuid(), Guid.NewGuid(), "usuario-1"));
        }

        [Fact]
        public void Crear_StockMinimoNegativo_LanzaExcepcion()
        {
            Assert.Throws<ArgumentException>(() => Ingrediente.Crear(
                "Harina", null, 10m, -1m, Guid.NewGuid(), Guid.NewGuid(), "usuario-1"));
        }

        [Fact]
        public void Crear_UsuarioCreacionVacio_LanzaExcepcion()
        {
            Assert.Throws<ArgumentException>(() => Ingrediente.Crear(
                "Harina", null, 10m, 5m, Guid.NewGuid(), Guid.NewGuid(), " "));
        }

        [Fact]
        public void AjustarStock_CantidadPositiva_IncrementaStockYFechaModificacion()
        {
            var ingrediente = CrearIngredienteValido();

            ingrediente.AjustarStock(20m);

            Assert.Equal(20m, ingrediente.StockActual);
            Assert.NotNull(ingrediente.FechaModificacion);
        }

        [Fact]
        public void AjustarStock_ResultadoNegativo_LanzaExcepcion()
        {
            var ingrediente = CrearIngredienteValido();
            ingrediente.AjustarStock(10m);

            Assert.Throws<InvalidOperationException>(() => ingrediente.AjustarStock(-15m));
        }

        [Fact]
        public void AjustarStock_DescuentoValido_DisminuyeStock()
        {
            var ingrediente = CrearIngredienteValido();
            ingrediente.AjustarStock(10m);

            ingrediente.AjustarStock(-4m);

            Assert.Equal(6m, ingrediente.StockActual);
        }

        [Fact]
        public void ActualizarPrecio_ValorNegativo_LanzaExcepcion()
        {
            var ingrediente = CrearIngredienteValido();

            Assert.Throws<ArgumentException>(() => ingrediente.ActualizarPrecio(-5m));
        }

        [Fact]
        public void ActualizarPrecio_ValorValido_ActualizaPrecioYFechaModificacion()
        {
            var ingrediente = CrearIngredienteValido();

            ingrediente.ActualizarPrecio(15.5m);

            Assert.Equal(15.5m, ingrediente.PrecioUnitario);
            Assert.NotNull(ingrediente.FechaModificacion);
        }

        [Fact]
        public void Desactivar_IngredienteActivo_QuedaInactivo()
        {
            var ingrediente = CrearIngredienteValido();

            ingrediente.Desactivar();

            Assert.False(ingrediente.Activo);
        }

        [Fact]
        public void Activar_IngredienteInactivo_QuedaActivo()
        {
            var ingrediente = CrearIngredienteValido();
            ingrediente.Desactivar();

            ingrediente.Activar();

            Assert.True(ingrediente.Activo);
        }

        [Fact]
        public void StockPorDebajoDelMinimo_StockMenorAlMinimo_RetornaTrue()
        {
            var ingrediente = CrearIngredienteValido(stockMinimo: 10m);
            ingrediente.AjustarStock(5m);

            Assert.True(ingrediente.StockPorDebajoDelMinimo());
        }

        [Fact]
        public void StockPorDebajoDelMinimo_StockSuficiente_RetornaFalse()
        {
            var ingrediente = CrearIngredienteValido(stockMinimo: 10m);
            ingrediente.AjustarStock(15m);

            Assert.False(ingrediente.StockPorDebajoDelMinimo());
        }
    }
}
