using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
	public interface ILifetimer
	{
		void OnLifetimeExpired();
	}

	public class LifetimerController : ITickable
	{
		private readonly LinkedList<Timer> _lifetimers = new LinkedList<Timer>();

		public Key Start( ILifetimer lifetimer, float duration )
		{
			var node = new LinkedListNode<Timer>( new Timer( lifetimer, duration ) );
			_lifetimers.AddLast( node );

			return new Key( node );
		}

		public void Remove( Key key )
		{
			_lifetimers.Remove( key.Node );
		}

		public void Tick()
		{
			if ( _lifetimers.Count <= 0 )
			{
				return;
			}

			var timerNode = _lifetimers.First;
			while ( timerNode != null )
			{
				if ( !timerNode.Value.Tick() )
				{
					_lifetimers.Remove( timerNode );
				}

				timerNode = timerNode.Next;
			}
		}

		public class Timer
		{
			public float Percentage => Mathf.Clamp01( 1 - ((_expirationTime - Time.timeSinceLevelLoad) / _duration) );

			private ILifetimer _lifetimer;
			private float _expirationTime;
			private float _duration;

			public Timer( ILifetimer lifetimer, float duration )
			{
				_lifetimer = lifetimer;
				_expirationTime = Time.timeSinceLevelLoad + duration;
				_duration = duration;
			}

			/// <returns>True while still alive.</returns>
			public bool Tick()
			{
				bool isLiving = _expirationTime > Time.timeSinceLevelLoad;
				if ( !isLiving )
				{
					_lifetimer.OnLifetimeExpired();
				}

				return isLiving;
			}
		}

		public class Key
		{
			public float Percentage => Node.Value.Percentage;
			public LinkedListNode<Timer> Node { get; }

			public Key( LinkedListNode<Timer> node )
			{
				Node = node;
			}
		}
	}
}