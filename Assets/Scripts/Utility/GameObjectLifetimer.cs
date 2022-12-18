using UnityEngine;

namespace Minipede.Utility
{
	public class GameObjectLifetimer : ILifetimer
	{
		private readonly GameObject _gameObject;

		public GameObjectLifetimer( GameObject gameObject )
		{
			_gameObject = gameObject;
		}

		public void OnLifetimeExpired()
		{
			GameObject.Destroy( _gameObject );
		}
	}
}