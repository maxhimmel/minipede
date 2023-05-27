using Minipede.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Installers
{
	public class ShipShrapnelSettings : HaulableSettings
	{
		[PropertyOrder( -1 )]
		[HideLabel, BoxGroup( "Shrapnel" )]
		[SerializeField] private ShipShrapnel.Settings _shrapnel;

		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.BindInstance( _shrapnel )
				.AsSingle()
				.WhenInjectedInto<ShipShrapnel>();
		}
	}
}