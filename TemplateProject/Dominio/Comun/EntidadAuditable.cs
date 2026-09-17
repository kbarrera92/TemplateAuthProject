namespace TemplateProject.Dominio.Comun
{
    public abstract class EntidadAuditable : EntidadBase
    {
        public DateTime FechaCreacion { get; private set; }
        public DateTime? FechaModificacion { get; private set; }
        public string UsuarioCreacionId { get; private set; } = null!;

        protected void RegistrarCreacion(string usuarioCreacionId, TimeProvider tiempo)
        {
            if (string.IsNullOrWhiteSpace(usuarioCreacionId))
                throw new ExcepcionDominio("El usuario de creación es obligatorio.");

            UsuarioCreacionId = usuarioCreacionId;
            FechaCreacion = tiempo.GetUtcNow().UtcDateTime;
        }

        protected void RegistrarModificacion(TimeProvider tiempo)
        {
            FechaModificacion = tiempo.GetUtcNow().UtcDateTime;
        }
    }
}
