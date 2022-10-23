using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
    public interface ISpawnPoint
    {
        Vector2 Position { get; }
        Quaternion Rotation { get; }
        Transform Container { get; }

        public TransformData ToData()
        {
            return new TransformData( Position, Rotation, Container );
        }
    }
}
