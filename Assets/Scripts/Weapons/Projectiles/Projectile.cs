using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public partial class Projectile : MonoBehaviour,
		IListener<DamagedSignal>
	{
		public event System.Action<Projectile> Destroyed;

		private Rigidbody2D _body;

		[Inject]
		public void Construct( Rigidbody2D body )
		{
			_body = body;
		}

		public void Launch( Vector2 impulse )
		{
			Launch( impulse, 0 );
		}

		public void Launch( Vector2 impulse, float torque )
		{
			if ( impulse != Vector2.zero )
			{
				_body.AddForce( impulse, ForceMode2D.Impulse );
			}
			if ( torque != 0 )
			{
				_body.AddTorque( torque, ForceMode2D.Impulse );
			}
		}

		public void Notify( DamagedSignal message )
		{
			Destroy( gameObject );
		}

		private void OnDestroy()
		{
			Destroyed?.Invoke( this );
		}
	}
}
