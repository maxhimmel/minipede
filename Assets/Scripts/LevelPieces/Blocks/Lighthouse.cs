using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Lighthouse : Block
	{
		private CleansedArea.Factory _cleansedAreaFactory;

		private CleansedArea _cleansedArea;

		[Inject]
		public void Construct( CleansedArea.Factory cleansedAreaFactory )
		{
			_cleansedAreaFactory = cleansedAreaFactory;
		}

		public void Equip( Beacon beacon )
		{
			beacon.StopFollowing();
			beacon.Equip( _body );
			beacon.Gun.Reload();

			var cleansedAreaPrefab = beacon.CleansedAreaProvider.GetAsset();
			_cleansedArea = _cleansedAreaFactory.Create( cleansedAreaPrefab, new Orientation( _body.position ) );
			_cleansedArea.Activate();
		}
	}
}
