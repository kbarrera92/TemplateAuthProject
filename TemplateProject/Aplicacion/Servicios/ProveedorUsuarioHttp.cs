using TemplateProject.Dominio.Comun;

namespace TemplateProject.Aplicacion.Servicios
{
    public class ProveedorUsuarioHttp : IProveedorUsuario
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ProveedorUsuarioHttp(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string ObtenerUsuarioId()
        {
            var usuarioId = httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value;

            if (string.IsNullOrWhiteSpace(usuarioId))
                throw new ExcepcionDominio("No fue posible determinar el usuario autenticado.");

            return usuarioId;
        }
    }
}
