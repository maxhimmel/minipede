using UnityEngine;

namespace Minipede.Utility
{
    public struct TransformData
    {
        public Vector2 Position;
        public Quaternion Rotation;
        public Transform Parent;

        public TransformData( Vector2 position, Quaternion rotation, Transform parent )
        {
            Position = position;
            Rotation = rotation;
            Parent = parent;
        }

        public TransformData( Vector2 position, Quaternion rotation ) : this( position, rotation, null )
        {
        }

        public TransformData( Vector2 position ) : this( position, Quaternion.identity )
        {
        }
    }
}
