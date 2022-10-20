using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Movement
{
    public interface IFollower
    {
        public Rigidbody2D Body { get; }

        public void StartFollowing( Rigidbody2D target );

        public class Factory : PlaceholderFactory<IFollower> { }
    }
}
