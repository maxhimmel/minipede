using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public interface IFollower : IFixedTickable
	{
		Rigidbody2D Body { get; }
		bool IsFollowing { get; }
		Vector2 Target { get; }

		void SnapToCollector( Rigidbody2D collector );
		void Follow( Rigidbody2D target );
		void StopFollowing();
	}
}