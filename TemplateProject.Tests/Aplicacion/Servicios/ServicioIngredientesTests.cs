using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.Datos;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using TemplateProject.DTOs.Ingredientes;
using Xunit;

namespace TemplateProject.Tests.Aplicacion.Servicios
{
    public class ServicioIngredientesTests : BaseServicioTests
    {
        private static async Task<(CategoriaIngrediente categoria, UnidadMedida unidad)> SembrarCatalogos(
            ApplicationDbContext contexto, FakeTimeProvider tiempo, bool categoriaActiva = true, bool unidadActiva = true)
        {
            var categoria = CategoriaIngrediente.Crear("Lácteos", null, "usuario-1", tiempo);
            var unidad = UnidadMedida.Crear("Kilogramo", "kg", "usuario-1", tiempo);

            if (!categoriaActiva) categoria.Desactivar(tiempo);
            if (!unidadActiva) unidad.Desactivar(tiempo);

            contexto.CategoriasIngredientes.Add(categoria);
            contexto.UnidadesMedida.Add(unidad);
            await contexto.SaveChangesAsync();

            return (categoria, unidad);
        }

        private static CrearIngredienteDTO CrearDtoValido(Guid categoriaId, Guid unidadId, string nombre = "Harina") => new()
        {
            Nombre = nombre,
            Descripcion = "Harina de trigo",
            PrecioUnitario = 10m,
            StockMinimo = 5m,
            CategoriaIngredienteId = categoriaId,
            UnidadMedidaId = unidadId
        };

        [Fact]
        public async Task Crear_DatosValidos_PersisteYSellaAutoria()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (categoria, unidad) = await SembrarCatalogos(contexto, tiempo);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario("usuario-1"));

            var resultado = await servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id));

            Assert.NotEqual(Guid.Empty, resultado.Id);
            Assert.Equal("usuario-1", resultado.UsuarioCreacionId);
            Assert.Equal("Lácteos", resultado.CategoriaIngredienteNombre);
            Assert.Equal("Kilogramo", resultado.UnidadMedidaNombre);
            Assert.Equal(1, await contexto.Ingredientes.CountAsync());
        }

        [Fact]
        public async Task Crear_NombreDuplicado_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (categoria, unidad) = await SembrarCatalogos(contexto, tiempo);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario());
            await servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id));

            await Assert.ThrowsAsync<ExcepcionDominio>(() =>
                servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id)));
        }

        [Fact]
        public async Task Crear_CategoriaInexistente_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (_, unidad) = await SembrarCatalogos(contexto, tiempo);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario());

            await Assert.ThrowsAsync<ExcepcionDominio>(() =>
                servicio.Crear(CrearDtoValido(Guid.NewGuid(), unidad.Id)));
        }

        [Fact]
        public async Task Crear_CategoriaInactiva_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (categoria, unidad) = await SembrarCatalogos(contexto, tiempo, categoriaActiva: false);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario());

            await Assert.ThrowsAsync<ExcepcionDominio>(() =>
                servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id)));
        }

        [Fact]
        public async Task Crear_UnidadMedidaInactiva_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (categoria, unidad) = await SembrarCatalogos(contexto, tiempo, unidadActiva: false);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario());

            await Assert.ThrowsAsync<ExcepcionDominio>(() =>
                servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id)));
        }

        [Fact]
        public async Task ObtenerPorId_IdInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.ObtenerPorId(Guid.NewGuid());

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Desactivar_IngredienteExistente_QuedaInactivoSinBorrarse()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (categoria, unidad) = await SembrarCatalogos(contexto, tiempo);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario());
            var ingrediente = await servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id));

            var resultado = await servicio.Desactivar(ingrediente.Id);

            Assert.True(resultado);
            var enBd = await contexto.Ingredientes.FindAsync(ingrediente.Id);
            Assert.NotNull(enBd);
            Assert.False(enBd!.Activo);
        }

        [Fact]
        public async Task Desactivar_IdInexistente_RetornaFalse()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.Desactivar(Guid.NewGuid());

            Assert.False(resultado);
        }

        [Fact]
        public async Task Listar_FiltraPorActivo()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (categoria, unidad) = await SembrarCatalogos(contexto, tiempo);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario());
            var activo = await servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id, "Harina"));
            var inactivo = await servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id, "Azúcar"));
            await servicio.Desactivar(inactivo.Id);

            var soloActivos = await servicio.Listar(activo: true);

            Assert.Single(soloActivos);
            Assert.Equal(activo.Id, soloActivos[0].Id);
        }

        [Fact]
        public async Task AjustarStock_ResultadoNegativo_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (categoria, unidad) = await SembrarCatalogos(contexto, tiempo);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario());
            var ingrediente = await servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id));

            await Assert.ThrowsAsync<ExcepcionDominio>(() => servicio.AjustarStock(ingrediente.Id, -1m));
        }

        [Fact]
        public async Task AjustarStock_ValorValido_ActualizaStockYFechaModificacion()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var (categoria, unidad) = await SembrarCatalogos(contexto, tiempo);
            var servicio = new ServicioIngredientes(contexto, tiempo, CrearProveedorUsuario());
            var ingrediente = await servicio.Crear(CrearDtoValido(categoria.Id, unidad.Id));
            var instanteAjuste = InstanteInicial.AddHours(2);
            tiempo.SetUtcNow(instanteAjuste);

            var resultado = await servicio.AjustarStock(ingrediente.Id, 20m);

            Assert.NotNull(resultado);
            Assert.Equal(20m, resultado!.StockActual);
            Assert.Equal(instanteAjuste.UtcDateTime, resultado.FechaModificacion);
        }

        [Fact]
        public async Task AjustarStock_IdInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioIngredientes(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.AjustarStock(Guid.NewGuid(), 10m);

            Assert.Null(resultado);
        }
    }
}
