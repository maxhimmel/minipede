using System.Collections.Generic;
using System.Linq;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Resources/Inventory" )]
	public class InventorySettings : ScriptableObjectInstaller
	{
		[FoldoutGroup( "Resource Types" ), LabelText( "Core" ), TableList]
		[SerializeField] private ResourceGroup[] _coreResources;
		[FoldoutGroup( "Resource Types" ), LabelText( "Extra" ), TableList]
		[SerializeField] private ResourceGroup[] _extraResources;

		[FoldoutGroup( "Inventory" ), HideLabel]
		[SerializeField] private Inventory.Settings _inventory;

		public override void InstallBindings()
		{
			var allResources = _coreResources.Concat( _extraResources );

			Container.Bind<ResourceType[]>()
				.FromInstance( _coreResources.Select( group => group.Resource ).ToArray() )
				.AsSingle();

			/* --- */

			foreach ( var group in allResources )
			{
				Container.Bind<Beacon.Factory>()
					.AsCached()
					.WithArguments( group.Resource, group.BeaconPrefab );
			}

			Container.Bind<BeaconFactoryBus>()
				.AsSingle();

			/* --- */

			Container.BindInterfacesAndSelfTo<Inventory>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<Inventory>()
						.AsSingle()
						.WithArguments( _inventory );

					subContainer.Bind<Wallet>()
						.AsSingle();
				} )
				.AsSingle();

			Container.Bind<EquippedGunModel>()
				.AsSingle();

			/* --- */

			DeclareSignals( allResources );
		}

		private void DeclareSignals( IEnumerable<ResourceGroup> allResources )
		{
			foreach ( var group in allResources )
			{
				Container.DeclareSignal<ResourceAmountChangedSignal>()
					.WithId( group.Resource )
					.OptionalSubscriber();

				Container.DeclareSignal<BeaconCreationStateChangedSignal>()
					.WithId( group.Resource )
					.OptionalSubscriber();
			}

			Container.DeclareSignal<BeaconEquippedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BeaconUnequippedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<CreateBeaconSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BeaconTypeSelectedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BeaconCreationProcessChangedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<ToggleInventorySignal>()
				.OptionalSubscriber();
		}

		[System.Serializable]
		private class ResourceGroup
		{
			public ResourceType Resource;
			public Beacon BeaconPrefab;
		}
	}
}
