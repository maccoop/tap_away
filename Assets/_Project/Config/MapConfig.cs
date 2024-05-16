using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapConfig", menuName = "config/MapConfig")]
public class MapConfig : SingletonScriptableObject<MapConfig>
{
    public Vector3 size;
    public Vector3 position;
    public float margin;

    public Vector3 GetPosition(Int3 value)
    {
        var result = new Vector3();
        result.x = position.x + size.x * value.x + value.x * margin;
        result.y = position.y + size.y * value.y + value.y * margin;
        result.z = position.z + size.z * value.z + value.z * margin;
        return result;
    }

    public Int3 GetInt3(Vector3 target)
    {
        Int3 result;
        result.x = Mathf.RoundToInt((target.x - position.x) / (size.x + margin));
        result.y = Mathf.RoundToInt((target.y - position.y) / (size.y + margin));
        result.z = Mathf.RoundToInt((target.z - position.z) / (size.z + margin));
        return result;
    }

    public Int3 GetInt3(Vector3 target, int index)
    {
        Int3 result;
        if (target.x != 0)
        {
            result.x = Mathf.RoundToInt((target.x - position.x) / (size.x + margin));
        }
        else
        {
            result.x = index;
        }
        if (target.y != 0)
        {
            result.y = Mathf.RoundToInt((target.y - position.y) / (size.y + margin));
        }
        else
        {

            result.y = index;
        }
        if (target.z != 0)
        {
            result.z = Mathf.RoundToInt((target.z - position.z) / (size.z + margin));
        }
        else
        {
            result.z = index;
        }
        return result;
    }

    public Int3 GetInt3(Vector3 target, Vector3 direction, int index)
    {
        Int3 result;
        result.x = Mathf.RoundToInt((target.x - position.x) / (size.x + margin));
        result.y = Mathf.RoundToInt((target.y - position.y) / (size.y + margin));
        result.z = Mathf.RoundToInt((target.z - position.z) / (size.z + margin));
        if (direction.x == 0)
        {
            result.x = index;
        }
        else if (direction.y == 0)
        {
            result.y = index;
        }
        else
        {
            result.z = index;
        }
        return result;
    }

    internal Vector3 GetPositionRelative(Vector3 position)
    {
        return GetPosition(GetInt3(position));
    }
    internal Vector3 GetPositionRelative(Vector3 position, Vector3 direction, int indexZ)
    {
        var result = GetPosition(GetInt3(position));
        if (direction.x != 0)
        {
            result.x = indexZ;
        }
        else if (direction.y != 0)
        {
            result.y = indexZ;
        }
        else
        {
            result.z = indexZ;
        }
        return result;
    }
}

[System.Serializable]
public struct Int3
{
    public int x, y, z;

    public Int3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public static readonly Int3 Left = new Int3(-1, 0, 0);
    public static readonly Int3 Right = new Int3(1, 0, 0);
    public static readonly Int3 Up = new Int3(0, 1, 0);
    public static readonly Int3 Down = new Int3(0, -1, 0);
    public static readonly Int3 Forward = new Int3(0, 0, 1);
    public static readonly Int3 Back = new Int3(0, 0, -1);
    public static readonly Int3 zeroX = new Int3(int.MaxValue, 1, 1);
    public static readonly Int3 zeroY = new Int3(1, int.MaxValue, 1);
    public static readonly Int3 zeroZ = new Int3(1, 1, int.MaxValue);
    public static readonly Int3 zero = new Int3(0, 0, 0);
    public static readonly Int3 one = new Int3(1, 1, 1);

    public override string ToString()
    {
        return $"({x},{y},{z})";
    }

    public static Int3 operator +(Int3 a, Int3 b)
    {
        return new Int3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static Int3 operator -(Int3 a, Int3 b)
    {
        return a + b * -1;
    }
    public static Int3 operator *(Int3 a, int b)
    {
        return new Int3(a.x * b, a.y * b, a.z * b);
    }
    public static Int3 operator *(Int3 a, Int3 b)
    {
        return new Int3(a.x * b.x, a.y * b.y, a.z * b.z);
    }
    public static Int3 operator /(Int3 a, int b)
    {
        return new Int3(a.x / b, a.y / b, a.z / b);
    }
    public static Int3 operator ^(Int3 a, int b)
    {
        return new Int3(a.x ^ b, a.y ^ b, a.z ^ b);
    }

    public static bool operator >(Int3 a, Int3 b)
    {
        return a.x > b.x || a.y > b.y || a.z > b.z;
    }
    public static bool operator <(Int3 a, Int3 b)
    {
        return a.x < b.x || a.y < b.y || a.z < b.z;
    }

    public static bool operator <=(Int3 a, Int3 b)
    {
        return a.x <= b.x || a.y <= b.y || a.z <= b.z;
    }
    public static bool operator >=(Int3 a, Int3 b)
    {
        return a.x >= b.x || a.y >= b.y || a.z >= b.z;
    }
    public static bool operator ==(Int3 a, Int3 b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }
    public static bool operator !=(Int3 a, Int3 b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj.GetType().Equals(typeof(Int3)))
        {
            return this == (Int3)obj;
        }
        return false;
    }

    public bool Contains(Int3 b)
    {
        return (this.x == b.x || b.x == int.MaxValue)
            && (this.y == b.y || b.y == int.MaxValue)
            && (this.z == b.z || b.z == int.MaxValue);
    }
}
