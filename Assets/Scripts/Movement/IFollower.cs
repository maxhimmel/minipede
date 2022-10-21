using Minipede.Gameplay.Enemies;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Movement
{
    public interface IFollower
    {
        public Rigidbody2D Body { get; }

        public void StartFollowing( Rigidbody2D target );
    }
}
