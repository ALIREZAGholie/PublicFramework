﻿using System.Reflection;

namespace Webgostar.Framework.Domain.ValueObjects;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class IgnoreMemberAttribute : Attribute;

public abstract class ValueObject : IEquatable<ValueObject>
{
    private List<PropertyInfo>? _properties;
    private List<FieldInfo>? _fields;

    public static bool operator ==(ValueObject obj1, ValueObject obj2)
    {
        return obj1?.Equals(obj2) ?? Equals(obj2, null);
    }

    public static bool operator !=(ValueObject obj1, ValueObject obj2)
    {
        return !(obj1 == obj2);
    }

    public bool Equals(ValueObject? obj)
    {
        return Equals(obj as object);
    }

    public override bool Equals(object? obj)
    {
        return obj != null && GetType() == obj.GetType() && (GetProperties().All(p => PropertiesAreEqual(obj, p))
                                                             && GetFields().All(f => FieldsAreEqual(obj, f)));
    }

    private bool PropertiesAreEqual(object obj, PropertyInfo p)
    {
        return Equals(p.GetValue(this, null), p.GetValue(obj, null));
    }

    private bool FieldsAreEqual(object obj, FieldInfo f)
    {
        return Equals(f.GetValue(this), f.GetValue(obj));
    }

    private IEnumerable<PropertyInfo> GetProperties()
    {
        return _properties ??= GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(p => !Attribute.IsDefined(p, typeof(IgnoreMemberAttribute))).ToList();
    }

    private IEnumerable<FieldInfo> GetFields()
    {
        return _fields ??= GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)
            .Where(f => !Attribute.IsDefined(f, typeof(IgnoreMemberAttribute))).ToList();
    }

    public override int GetHashCode()
    {
        unchecked //allow override
        {
            var hash = 17;
            foreach (var prop in GetProperties())
            {
                var value = prop.GetValue(this, null);
                if (value != null) hash = HashValue(hash, value);
            }

            foreach (var field in GetFields())
            {
                var value = field.GetValue(this);
                if (value != null) hash = HashValue(hash, value);
            }

            return hash;
        }
    }

    private int HashValue(int seed, object? value)
    {
        var currentHash = value?.GetHashCode() ?? 0;

        return seed * 23 + currentHash;
    }
}