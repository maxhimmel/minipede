using UnityEngine;

namespace Minipede.Utility
{
	public class Lifetimer : MonoBehaviour
	{
		private bool _canExpire;
		private float _expirationTime;

		public void StartLifetime( float duration )
		{
			_canExpire = true;
			_expirationTime = Time.timeSinceLevelLoad + duration;
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