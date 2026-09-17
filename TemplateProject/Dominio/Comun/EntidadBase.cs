namespace TemplateProject.Dominio.Comun
{
    public abstract class EntidadBase
    {
        public Guid Id { get; protected set; }

        public override bool Equals(object? obj)
        {
            if (obj is not EntidadBase otra) return false;
            if (ReferenceEquals(this, otra)) return true;
            if (GetType() != otra.GetType()) return false;
            if (Id == Guid.Empty || otra.Id == Guid.Empty) return false;
            return Id == otra.Id;
        }

        public override int GetHashCode() => HashCode.Combine(GetType(), Id);

        public static bool operator ==(EntidadBase? a, EntidadBase? b) => Equals(a, b);
        public static bool operator !=(EntidadBase? a, EntidadBase? b) => !Equals(a, b);
    }
}
