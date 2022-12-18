using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
	public class Lifetimer : MonoBehaviour
	{
		private float _lifetime;
		private bool _canExpire;
		private float _expirationTime;

		[Inject]
		public void Construct( float lifetime )
		{
			_lifetime = lifetime;
		}

		public void StartLifetime()
		{
			_canExpire = true;
			_expirationTime = Time.timeSinceLevelLoad + _lifetime;
		}

		private void Update()
		{
			if ( !_canExpire )
			{
				return;
			}

			if ( _expirationTime <= Time.timeSinceLevelLoad )
			{
				Destroy( gameObject );
			}
		}

		[System.Serializable]
		public struct Settings
		{
			public float Lifetime;
		}

		public class Factory : UnityPrefabFactory<Lifetimer> { }
	}
}