using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Lighthouse : Block
	{
		public void Equip( Beacon beacon )
		{
			beacon.StopFollowing();
			beacon.Equip( _body );
			beacon.Gun.Reload();
		}

		public override void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			base.OnSpawned( placement, pool );

			_signalBus.TryFireId( "Spawned", new FxSignal( _body.position, Vector2.up, _body.transform ) );
		}
	}
}
