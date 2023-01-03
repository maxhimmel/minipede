using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Lighthouse : Block
	{
		private Rigidbody2D _body;
		private CleansedArea.Factory _cleansedAreaFactory;

		private CleansedArea _cleansedArea;

		[Inject]
		public void Construct( Rigidbody2D body,
			CleansedArea.Factory cleansedAreaFactory )
		{
			_body = body;
			_cleansedAreaFactory = cleansedAreaFactory;
		}

		public void Equip( Beacon beacon )
		{
			beacon.StopFollowing();
			beacon.Equip( _body );

			var cleansedAreaPrefab = beacon.CleansedAreaProvider.GetAsset();
			_cleansedArea = _cleansedAreaFactory.Create( cleansedAreaPrefab, new Orientation( _body.position ) );
		}

		protected override void OnTakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			base.OnTakeDamage( instigator, causer, data );

			// TODO: This wouldn't be checked here:
				// It'd be placed within the FluffyEnemyINVOKER.
				// Then within there it could do some casting to some interface to see if it can be "disabled."
			//if ( data is FluffyEnemy )
			//{
			//	_cleansedArea.Deactivate();
			//}
		}
	}
}
