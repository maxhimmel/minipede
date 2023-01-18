using UnityEngine;

namespace Minipede.Utility
{
	public class Lifetimer
	{
		public float Percentage => _lifetimer;

		private bool _canExpire;
		private float _duration;
		private float _lifetimer;

		public Lifetimer() { }
		public Lifetimer( float duration )
		{
			SetDuration( duration );
		}

		/// <summary>
		/// Resets counter to zero and starts count down.
		/// </summary>
		public void Reset()
		{
			StartLifetime( _duration );
		}

		public virtual void StartLifetime( float duration )
		{
			_lifetimer = 0;

			SetDuration( duration );
			Resume();
		}

		public void SetDuration( float duration )
		{
			_duration = duration;
		}

		public virtual void Resume()
		{
			_canExpire = true;
		}

		public virtual void Pause()
		{
			_canExpire = false;
		}

		/// <returns>True while timer is alive.</returns>
		public virtual bool Tick()
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