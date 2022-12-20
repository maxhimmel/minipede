using UnityEngine;

namespace Minipede.Utility
{
	public class Lifetimer
	{
		public float Percentage => _lifetimer;

		private bool _canExpire;
		private float _duration;
		private float _lifetimer;

		/// <summary>
		/// Resets counter to zero and starts count down.
		/// </summary>
		public void Reset()
		{
			StartLifetime( _duration );
		}

		public void StartLifetime( float duration )
		{
			_canExpire = true;
			_duration = duration;
			_lifetimer = 0;
		}

		public void Pause()
		{
			_canExpire = false;
		}

		public void Resume()
		{
			_canExpire = true;
		}

		/// <returns>True while timer is alive.</returns>
		public bool Tick()
		{
			if ( !_canExpire )
			{
				return true;
			}

			_lifetimer += Time.deltaTime / _duration;
			return _lifetimer < 1;
		}
	}
}