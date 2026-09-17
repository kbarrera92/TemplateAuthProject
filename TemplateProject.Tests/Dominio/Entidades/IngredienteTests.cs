using Microsoft.Extensions.Time.Testing;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using Xunit;

namespace TemplateProject.Tests.Dominio.Entidades
{
    public class IngredienteTests
    {
        private static readonly DateTimeOffset InstanteInicial = new(2026, 9, 17, 6, 0, 0, TimeSpan.Zero);

        private static Ingrediente CrearIngredienteValido(
            FakeTimeProvider tiempo,
            decimal precioUnitario = 10m,
            decimal stockMinimo = 5m,
            Guid categoriaIngredienteId = default,
            Guid unidadMedidaId = default)
        {
            return Ingrediente.Crear(
                nombre: "Harina",
                descripcion: "Harina de trigo",
                precioUnitario: precioUnitario,
                stockMinimo: stockMinimo,
                categoriaIngredienteId: categoriaIngredienteId == default ? Guid.NewGuid() : categoriaIngredienteId,
                unidadMedidaId: unidadMedidaId == default ? Guid.NewGuid() : unidadMedidaId,
                usuarioCreacionId: "usuario-1",
                tiempo: tiempo);
        }

        private static FakeTimeProvider CrearReloj() => new(InstanteInicial);

        [Fact]
        public void Crear_DatosValidos_InicializaConStockEnCeroYActivo()
        {
            var tiempo = CrearReloj();

            var ingrediente = CrearIngredienteValido(tiempo);

            Assert.Equal("Harina", ingrediente.Nombre);
            Assert.True(ingrediente.Activo);
            Assert.Equal(0m, ingrediente.StockActual);
            Assert.NotEqual(Guid.Empty, ingrediente.Id);
            Assert.Equal("usuario-1", ingrediente.UsuarioCreacionId);
            Assert.Equal(InstanteInicial.UtcDateTime, ingrediente.FechaCreacion);
            Assert.Null(ingrediente.FechaModificacion);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Crear_NombreInvalido_LanzaExcepcion(string? nombre)
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Ingrediente.Crear(
                nombre!, "desc", 10m, 5m, Guid.NewGuid(), Guid.NewGuid(), "usuario-1", tiempo));
        }

        [Fact]
        public void Crear_PrecioNegativo_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Ingrediente.Crear(
                "Harina", null, -1m, 5m, Guid.NewGuid(), Guid.NewGuid(), "usuario-1", tiempo));
        }

        [Fact]
        public void Crear_StockMinimoNegativo_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Ingrediente.Crear(
                "Harina", null, 10m, -1m, Guid.NewGuid(), Guid.NewGuid(), "usuario-1", tiempo));
        }

        [Fact]
        public void Crear_UsuarioCreacionVacio_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Ingrediente.Crear(
                "Harina", null, 10m, 5m, Guid.NewGuid(), Guid.NewGuid(), " ", tiempo));
        }

        [Fact]
        public void Crear_CategoriaVacia_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Ingrediente.Crear(
                "Harina", null, 10m, 5m, Guid.Empty, Guid.NewGuid(), "usuario-1", tiempo));
        }

        [Fact]
        public void Crear_UnidadMedidaVacia_LanzaExcepcion()
        {
            var tiempo = CrearReloj();

            Assert.Throws<ExcepcionDominio>(() => Ingrediente.Crear(
                "Harina", null, 10m, 5m, Guid.NewGuid(), Guid.Empty, "usuario-1", tiempo));
        }

        [Fact]
        public void AjustarStock_CantidadPositiva_IncrementaStockYRegistraFechaModificacion()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);
            var instanteAjuste = InstanteInicial.AddHours(2);
            tiempo.SetUtcNow(instanteAjuste);

            ingrediente.AjustarStock(20m, tiempo);

            Assert.Equal(20m, ingrediente.StockActual);
            Assert.Equal(instanteAjuste.UtcDateTime, ingrediente.FechaModificacion);
        }

        [Fact]
        public void AjustarStock_ResultadoNegativo_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);
            ingrediente.AjustarStock(10m, tiempo);

            Assert.Throws<ExcepcionDominio>(() => ingrediente.AjustarStock(-15m, tiempo));
        }

        [Fact]
        public void AjustarStock_DescuentoValido_DisminuyeStock()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);
            ingrediente.AjustarStock(10m, tiempo);

            ingrediente.AjustarStock(-4m, tiempo);

            Assert.Equal(6m, ingrediente.StockActual);
        }

        [Fact]
        public void ActualizarNombre_ValorValido_ActualizaYRecortaEspacios()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);

            ingrediente.ActualizarNombre("  Harina integral  ", tiempo);

            Assert.Equal("Harina integral", ingrediente.Nombre);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ActualizarNombre_ValorInvalido_LanzaExcepcion(string nombre)
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => ingrediente.ActualizarNombre(nombre, tiempo));
        }

        [Fact]
        public void ActualizarDescripcion_PermiteNulo()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);

            ingrediente.ActualizarDescripcion(null, tiempo);

            Assert.Null(ingrediente.Descripcion);
        }

        [Fact]
        public void ActualizarStockMinimo_ValorValido_Actualiza()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo, stockMinimo: 5m);

            ingrediente.ActualizarStockMinimo(8m, tiempo);

            Assert.Equal(8m, ingrediente.StockMinimo);
        }

        [Fact]
        public void ActualizarStockMinimo_ValorNegativo_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => ingrediente.ActualizarStockMinimo(-1m, tiempo));
        }

        [Fact]
        public void CambiarCategoria_ValorValido_Actualiza()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);
            var nuevaCategoria = Guid.NewGuid();

            ingrediente.CambiarCategoria(nuevaCategoria, tiempo);

            Assert.Equal(nuevaCategoria, ingrediente.CategoriaIngredienteId);
        }

        [Fact]
        public void CambiarCategoria_ValorVacio_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => ingrediente.CambiarCategoria(Guid.Empty, tiempo));
        }

        [Fact]
        public void CambiarUnidadMedida_ValorValido_Actualiza()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);
            var nuevaUnidad = Guid.NewGuid();

            ingrediente.CambiarUnidadMedida(nuevaUnidad, tiempo);

            Assert.Equal(nuevaUnidad, ingrediente.UnidadMedidaId);
        }

        [Fact]
        public void CambiarUnidadMedida_ValorVacio_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => ingrediente.CambiarUnidadMedida(Guid.Empty, tiempo));
        }

        [Fact]
        public void ActualizarPrecio_ValorNegativo_LanzaExcepcion()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);

            Assert.Throws<ExcepcionDominio>(() => ingrediente.ActualizarPrecio(-5m, tiempo));
        }

        [Fact]
        public void ActualizarPrecio_ValorValido_ActualizaPrecioYRegistraFechaModificacion()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);
            var instanteCambio = InstanteInicial.AddDays(1);
            tiempo.SetUtcNow(instanteCambio);

            ingrediente.ActualizarPrecio(15.5m, tiempo);

            Assert.Equal(15.5m, ingrediente.PrecioUnitario);
            Assert.Equal(instanteCambio.UtcDateTime, ingrediente.FechaModificacion);
        }

        [Fact]
        public void Desactivar_IngredienteActivo_QuedaInactivo()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);

            ingrediente.Desactivar(tiempo);

            Assert.False(ingrediente.Activo);
        }

        [Fact]
        public void Activar_IngredienteInactivo_QuedaActivo()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo);
            ingrediente.Desactivar(tiempo);

            ingrediente.Activar(tiempo);

            Assert.True(ingrediente.Activo);
        }

        [Fact]
        public void StockPorDebajoDelMinimo_StockMenorAlMinimo_RetornaTrue()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo, stockMinimo: 10m);
            ingrediente.AjustarStock(5m, tiempo);

            Assert.True(ingrediente.StockPorDebajoDelMinimo());
        }

        [Fact]
        public void StockPorDebajoDelMinimo_StockIgualAlMinimo_RetornaTrue()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo, stockMinimo: 10m);
            ingrediente.AjustarStock(10m, tiempo);

            Assert.True(ingrediente.StockPorDebajoDelMinimo());
        }

        [Fact]
        public void StockPorDebajoDelMinimo_StockSuficiente_RetornaFalse()
        {
            var tiempo = CrearReloj();
            var ingrediente = CrearIngredienteValido(tiempo, stockMinimo: 10m);
            ingrediente.AjustarStock(15m, tiempo);

            Assert.False(ingrediente.StockPorDebajoDelMinimo());
        }
    }
}
