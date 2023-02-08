﻿using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

using BlockActor = Minipede.Gameplay.LevelPieces.Block;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Managers/LevelGeneration" )]
	public class LevelGenerationInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private GameplaySettings.Level _levelSettings;
		[SerializeField] private Block _blockSettings;
		[SerializeField] private PollutedAreaController.Settings _pollutionSettings;

		public override void InstallBindings()
		{
			Container.DeclareSignal<BlockSpawnedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<BlockDestroyedSignal>()
				.OptionalSubscriber();

			Container.DeclareSignal<PollutionLevelChangedSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<IWinStateChangedSignal>()
				.OptionalSubscriber();

			BindLevelGeneration();
			BindCleansing();
		}

		private void BindLevelGeneration()
		{
			Container.BindInstance( _levelSettings );

			Container.BindInstance( _blockSettings.Settings )
				.WhenInjectedInto<Mushroom>();

			/* --- */

			Container.Bind<PoisonTrailFactory>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
					PoisonTrailInstaller.Install( subContainer, _blockSettings.Poison )
				)
				.WithKernel()
				.AsCached()
				.WhenInjectedInto<PoisonMushroom>();

			Container.BindInstance( _levelSettings.PoisonDamage )
				.AsSingle()
				.WhenInjectedInto<DamageAOE>();

			/* --- */

			Container.BindInterfacesAndSelfTo<LevelMushroomHealer>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<LevelMushroomShifter>()
				.AsSingle();

			/* --- */

			Container.Bind<BlockActor.Factory>()
				.AsSingle()
				.WhenInjectedInto<BlockFactoryBus>();

			Container.Bind<BlockFactoryBus>()
				.AsSingle()
				.WithArguments( new List<BlockFactoryBus.PoolSettings>() {
					_blockSettings.Mushrooms.Standard,
					_blockSettings.Mushrooms.Poison,
					_blockSettings.Mushrooms.Flower
				} )
				.WhenInjectedInto<LevelGraph>();

			Container.Bind<MushroomProvider>()
				.AsSingle()
				.WithArguments( _blockSettings.Mushrooms );

			/* --- */

			Container.Bind<LevelGenerator>()
				.AsSingle();

			Container.Bind<LevelForeman>()
				.AsTransient();
		}

		private void BindCleansing()
		{
			Container.Bind<CleansedArea.Factory>()
				.AsSingle();

			/* --- */

			Container.Bind( typeof( PollutedAreaController.Settings ), typeof( IPollutionWinPercentage ) )
				.FromInstance( _pollutionSettings )
				.AsSingle();

			Container.BindInterfacesAndSelfTo<PollutedAreaController>()
				.AsSingle();
		}

		[System.Serializable]
		public struct Block
		{
			[HideLabel, FoldoutGroup( "Gameplay" )]
			public Mushroom.Settings Settings;
			[HideLabel, FoldoutGroup( "Mushrooms" )]
			public BlockFactoryBus.Settings Mushrooms;
			[HideLabel, FoldoutGroup( "Poison" )]
			public PoisonTrailInstaller.Settings Poison;
		}
	}
}