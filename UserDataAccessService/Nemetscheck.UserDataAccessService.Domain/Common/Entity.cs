namespace Nemetscheck.UserDataAccessService.Domain.Common;

public abstract class Entity<TId> : IEquatable<Entity<TId>>
    where TId : struct
{
    private bool IsTransient => EqualityComparer<TId>.Default.Equals(Id, default);

    public TId Id { get; private set; }

    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) => Equals(left, right);

    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !Equals(left, right);

    public bool Equals(Entity<TId>? other)
    {
        if (other is null || GetType() != other.GetType())
            return false;

        // not yet persisted entities have no identity, so only the same instance is equal
        if (IsTransient || other.IsTransient)
            return ReferenceEquals(this, other);

        return EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }

    public override bool Equals(object? obj) => obj is Entity<TId> other && Equals(other);

    public override int GetHashCode() =>
        IsTransient ? System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(this) : Id.GetHashCode();
}
