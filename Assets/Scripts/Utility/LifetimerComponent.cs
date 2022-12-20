using UnityEngine;

namespace Minipede.Utility
{
	public class LifetimerComponent : MonoBehaviour
	{
		private Lifetimer _lifetimer = new Lifetimer();

		public void StartLifetime( float duration )
		{
			_lifetimer.StartLifetime( duration );
		}

		private void Update()
		{
			if ( !_lifetimer.Tick() )
			{
				Destroy( gameObject );
			}
		}

		public class Factory : UnityPrefabFactory<LifetimerComponent> { }
	}
}