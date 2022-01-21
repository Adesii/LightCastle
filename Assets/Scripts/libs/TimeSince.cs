using System;
using UnityEngine;
public struct TimeSince : IEquatable<TimeSince>
{
    private float time;

    public float Absolute => time;

    public float Relative => this;

    public static implicit operator float(TimeSince ts)
    {
        return Time.time - ts.time;
    }

    public static implicit operator TimeSince(float ts)
    {
        TimeSince result = default;
        result.time = Time.time - ts;
        return result;
    }

    public static bool operator ==(TimeSince left, TimeSince right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TimeSince left, TimeSince right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj is TimeSince o)
        {
            return Equals(o);
        }
        return false;
    }

    public bool Equals(TimeSince o)
    {
        return time == o.time;
    }

    public override int GetHashCode()
    {
        return time.GetHashCode();
    }
}