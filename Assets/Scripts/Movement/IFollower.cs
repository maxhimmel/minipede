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

        //public class Factory : UnityFactory<IFollower> { }//UnityPlaceholderFactory<IFollower> { }
        //public class Factory2 : PlaceholderFactory<Vector2, IFollower> { }
        //public interface IFactory : Zenject.IFactory<Vector2, Quaternion, Transform, IFollower> { }
    }
}
