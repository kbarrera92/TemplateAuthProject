using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using TemplateProject.Aplicacion.Servicios;
using TemplateProject.Datos;

namespace TemplateProject.Tests.Aplicacion.Servicios
{
    public abstract class BaseServicioTests
    {
        protected static readonly DateTimeOffset InstanteInicial = new(2026, 9, 17, 6, 0, 0, TimeSpan.Zero);

        protected static ApplicationDbContext CrearContexto()
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(opciones);
        }

        protected static FakeTimeProvider CrearReloj() => new(InstanteInicial);

        protected static IProveedorUsuario CrearProveedorUsuario(string usuarioId = "usuario-1")
            => new ProveedorUsuarioFalso(usuarioId);

        private class ProveedorUsuarioFalso : IProveedorUsuario
        {
            private readonly string usuarioId;

            public ProveedorUsuarioFalso(string usuarioId)
            {
                this.usuarioId = usuarioId;
            }

            public string ObtenerUsuarioId() => usuarioId;
        }
    }
}
