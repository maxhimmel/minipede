using UnityEngine;

namespace Minipede.Utility
{
    public interface IOrientation
    {
        Vector2 Position { get; }
        Quaternion Rotation { get; }
        Transform Parent { get; }
    }
}
