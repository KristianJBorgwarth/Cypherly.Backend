﻿// ReSharper disable ConvertConstructorToMemberInitializers
namespace Cypherly.Domain.Common;

public abstract class Entity
{
    public Guid Id { get; init; }
    public DateTime LastModified { get; protected set; }
    public DateTime Created { get; protected set; }

    protected Entity()
    {
        Id = Guid.NewGuid();
        Created = DateTime.UtcNow;
        LastModified = DateTime.UtcNow;
    }

    public void UpdateLastModified()
    {
        LastModified = DateTime.UtcNow;
    }

    public override bool Equals(object obj)
    {
        var other = obj as Entity;
        if (ReferenceEquals(other, null))
        {
            return false;
        }
        if (ReferenceEquals(this, other))
        {
            return true;
        }
        if (GetType() != other.GetType())
        {
            return false;
        }

        return Id == other.Id; //identifier equality
    }

    public static bool operator ==(Entity a, Entity b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
        {
            return true;
        }
        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
        {
            return false;
        }
        return a.Equals(b);
    }

    public static bool operator !=(Entity a, Entity b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().ToString() + Id).GetHashCode();
    }

}