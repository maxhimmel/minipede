using UnityEngine;

namespace Minipede.Utility
{
    public struct Orientation : IOrientation
    {
        public Vector2 Position { get; }
        public Quaternion Rotation { get; }
        public Transform Parent { get; }

        public Orientation( Vector2 position, Quaternion rotation, Transform parent )
        {
            Position = position;
            Rotation = rotation;
            Parent = parent;
        }

        public Orientation( Vector2 position, Quaternion rotation ) : this( position, rotation, null )
        {
        }

        public Orientation( Vector2 position ) : this( position, Quaternion.identity )
        {
        }
    }
}
