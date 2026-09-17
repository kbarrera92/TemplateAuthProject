using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.Datos;
using TemplateProject.Dominio.Comun;
using TemplateProject.Dominio.Entidades;
using TemplateProject.DTOs.Platos;
using Xunit;

namespace TemplateProject.Tests.Aplicacion.Servicios
{
    public class ServicioPlatosTests : BaseServicioTests
    {
        private static async Task<Ingrediente> SembrarIngrediente(
            ApplicationDbContext contexto,
            FakeTimeProvider tiempo,
            string nombre = "Harina",
            decimal precioUnitario = 10m,
            bool activo = true)
        {
            var categoria = CategoriaIngrediente.Crear("Genérica", null, "usuario-1", tiempo);
            var unidad = UnidadMedida.Crear("Kilogramo", "kg", "usuario-1", tiempo);
            contexto.CategoriasIngredientes.Add(categoria);
            contexto.UnidadesMedida.Add(unidad);

            var ingrediente = Ingrediente.Crear(nombre, null, precioUnitario, 0m, categoria.Id, unidad.Id, "usuario-1", tiempo);
            if (!activo) ingrediente.Desactivar(tiempo);

            contexto.Ingredientes.Add(ingrediente);
            await contexto.SaveChangesAsync();

            return ingrediente;
        }

        private static CrearPlatoDTO CrearDtoValido(string nombre = "Bandeja paisa", decimal precioVenta = 25m) => new()
        {
            Nombre = nombre,
            Descripcion = "Plato típico",
            PrecioVenta = precioVenta
        };

        [Fact]
        public async Task Crear_DatosValidos_PersisteYSellaAutoria()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario("usuario-1"));

            var resultado = await servicio.Crear(CrearDtoValido());

            Assert.NotEqual(Guid.Empty, resultado.Id);
            Assert.Equal("usuario-1", resultado.UsuarioCreacionId);
            Assert.Empty(resultado.ItemsReceta);
            Assert.Equal(0m, resultado.CostoReceta);
            Assert.Equal(25m, resultado.MargenGanancia);
            Assert.Equal(1, await contexto.Platos.CountAsync());
        }

        [Fact]
        public async Task Crear_NombreDuplicado_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            await servicio.Crear(CrearDtoValido());

            await Assert.ThrowsAsync<ExcepcionDominio>(() => servicio.Crear(CrearDtoValido()));
        }

        [Fact]
        public async Task ObtenerPorId_IdInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioPlatos(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.ObtenerPorId(Guid.NewGuid());

            Assert.Null(resultado);
        }

        [Fact]
        public async Task Desactivar_PlatoExistente_QuedaInactivoSinBorrarse()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var plato = await servicio.Crear(CrearDtoValido());

            var resultado = await servicio.Desactivar(plato.Id);

            Assert.True(resultado);
            var enBd = await contexto.Platos.FindAsync(plato.Id);
            Assert.NotNull(enBd);
            Assert.False(enBd!.Activo);
        }

        [Fact]
        public async Task Desactivar_IdInexistente_RetornaFalse()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioPlatos(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.Desactivar(Guid.NewGuid());

            Assert.False(resultado);
        }

        [Fact]
        public async Task Listar_FiltraPorActivo()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var activo = await servicio.Crear(CrearDtoValido("Bandeja paisa"));
            var inactivo = await servicio.Crear(CrearDtoValido("Sancocho"));
            await servicio.Desactivar(inactivo.Id);

            var soloActivos = await servicio.Listar(activo: true);

            Assert.Single(soloActivos);
            Assert.Equal(activo.Id, soloActivos[0].Id);
        }

        [Fact]
        public async Task AgregarIngrediente_DatosValidos_CalculaCostoYMargen()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var ingrediente = await SembrarIngrediente(contexto, tiempo, precioUnitario: 4m);
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var plato = await servicio.Crear(CrearDtoValido(precioVenta: 25m));

            var resultado = await servicio.AgregarIngrediente(plato.Id, new AgregarItemRecetaDTO
            {
                IngredienteId = ingrediente.Id,
                Cantidad = 2m
            });

            Assert.NotNull(resultado);
            var item = Assert.Single(resultado!.ItemsReceta);
            Assert.Equal(ingrediente.Id, item.IngredienteId);
            Assert.Equal("Harina", item.IngredienteNombre);
            Assert.Equal(8m, item.Subtotal);
            Assert.Equal(8m, resultado.CostoReceta);
            Assert.Equal(17m, resultado.MargenGanancia);
        }

        [Fact]
        public async Task AgregarIngrediente_IngredienteInactivo_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var ingrediente = await SembrarIngrediente(contexto, tiempo, activo: false);
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var plato = await servicio.Crear(CrearDtoValido());

            await Assert.ThrowsAsync<ExcepcionDominio>(() => servicio.AgregarIngrediente(plato.Id, new AgregarItemRecetaDTO
            {
                IngredienteId = ingrediente.Id,
                Cantidad = 1m
            }));
        }

        [Fact]
        public async Task AgregarIngrediente_IngredienteInexistente_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var plato = await servicio.Crear(CrearDtoValido());

            await Assert.ThrowsAsync<ExcepcionDominio>(() => servicio.AgregarIngrediente(plato.Id, new AgregarItemRecetaDTO
            {
                IngredienteId = Guid.NewGuid(),
                Cantidad = 1m
            }));
        }

        [Fact]
        public async Task AgregarIngrediente_PlatoInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var ingrediente = await SembrarIngrediente(contexto, tiempo);
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());

            var resultado = await servicio.AgregarIngrediente(Guid.NewGuid(), new AgregarItemRecetaDTO
            {
                IngredienteId = ingrediente.Id,
                Cantidad = 1m
            });

            Assert.Null(resultado);
        }

        [Fact]
        public async Task AgregarIngrediente_IngredienteDuplicado_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var ingrediente = await SembrarIngrediente(contexto, tiempo);
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var plato = await servicio.Crear(CrearDtoValido());
            await servicio.AgregarIngrediente(plato.Id, new AgregarItemRecetaDTO { IngredienteId = ingrediente.Id, Cantidad = 1m });

            await Assert.ThrowsAsync<ExcepcionDominio>(() => servicio.AgregarIngrediente(plato.Id, new AgregarItemRecetaDTO
            {
                IngredienteId = ingrediente.Id,
                Cantidad = 2m
            }));
        }

        [Fact]
        public async Task ActualizarCantidadIngrediente_IngredienteExistente_RecalculaCosto()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var ingrediente = await SembrarIngrediente(contexto, tiempo, precioUnitario: 4m);
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var plato = await servicio.Crear(CrearDtoValido());
            await servicio.AgregarIngrediente(plato.Id, new AgregarItemRecetaDTO { IngredienteId = ingrediente.Id, Cantidad = 2m });

            var resultado = await servicio.ActualizarCantidadIngrediente(plato.Id, ingrediente.Id, new ActualizarItemRecetaDTO { Cantidad = 5m });

            Assert.NotNull(resultado);
            Assert.Equal(5m, resultado!.ItemsReceta.Single().Cantidad);
            Assert.Equal(20m, resultado.CostoReceta);
        }

        [Fact]
        public async Task ActualizarCantidadIngrediente_IngredienteNoPertenece_LanzaExcepcionDominio()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var plato = await servicio.Crear(CrearDtoValido());

            await Assert.ThrowsAsync<ExcepcionDominio>(() =>
                servicio.ActualizarCantidadIngrediente(plato.Id, Guid.NewGuid(), new ActualizarItemRecetaDTO { Cantidad = 1m }));
        }

        [Fact]
        public async Task ActualizarCantidadIngrediente_PlatoInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioPlatos(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.ActualizarCantidadIngrediente(Guid.NewGuid(), Guid.NewGuid(), new ActualizarItemRecetaDTO { Cantidad = 1m });

            Assert.Null(resultado);
        }

        [Fact]
        public async Task QuitarIngrediente_IngredienteExistente_LoElimina()
        {
            using var contexto = CrearContexto();
            var tiempo = CrearReloj();
            var ingrediente = await SembrarIngrediente(contexto, tiempo);
            var servicio = new ServicioPlatos(contexto, tiempo, CrearProveedorUsuario());
            var plato = await servicio.Crear(CrearDtoValido());
            await servicio.AgregarIngrediente(plato.Id, new AgregarItemRecetaDTO { IngredienteId = ingrediente.Id, Cantidad = 2m });

            var resultado = await servicio.QuitarIngrediente(plato.Id, ingrediente.Id);

            Assert.NotNull(resultado);
            Assert.Empty(resultado!.ItemsReceta);
            Assert.Equal(0m, resultado.CostoReceta);
        }

        [Fact]
        public async Task QuitarIngrediente_PlatoInexistente_RetornaNulo()
        {
            using var contexto = CrearContexto();
            var servicio = new ServicioPlatos(contexto, CrearReloj(), CrearProveedorUsuario());

            var resultado = await servicio.QuitarIngrediente(Guid.NewGuid(), Guid.NewGuid());

            Assert.Null(resultado);
        }
    }
}
