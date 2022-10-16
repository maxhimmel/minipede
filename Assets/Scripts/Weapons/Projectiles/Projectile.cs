using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public partial class Projectile : MonoBehaviour
	{
		public IOnDestroyedNotify DestroyedNotify { get; private set; }

		private Rigidbody2D _body;

		[Inject]
		public void Construct( Rigidbody2D body,
			IOnDestroyedNotify destroyedNotify )
		{
			_body = body;
			DestroyedNotify = destroyedNotify;
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
	}
}
