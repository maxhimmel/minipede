using Minipede.Gameplay;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Fx;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.StartSequence;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

using BeaconActor = Minipede.Gameplay.Treasures.Beacon;

namespace Minipede.Installers
{
	public class GameplaySettings : MonoInstaller
	{
		[SerializeField] private ResourceType[] _resourceTypes;
		[SerializeField] private Beacon _beaconSettings;
		[SerializeField] private LevelStartSequenceController.Settings _startGameSettings;
		[SerializeField] private EndGameController.Settings _endGameSettings;
		[SerializeField] private Audio _audioSettings;
		[SerializeField] private Camera _cameraSettings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<GameController>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<LevelStartSequenceController>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.BindInterfacesAndSelfTo<LevelStartSequenceController>()
						.AsSingle()
						.WithArguments( _startGameSettings );

					subContainer.BindInterfacesAndSelfTo<CameraToggler>()
						.AsSingle()
						.WithArguments( _startGameSettings.Camera );
				} )
				.AsSingle();

			Container.BindInterfacesAndSelfTo<EndGameController>()
				.AsSingle()
				.WithArguments( _endGameSettings );

			BindCameraSystems();
			BindTreasure();
			BindAudio();
			BindFxPool();

			DeclareSignals();
		}

		private void BindCameraSystems()
		{
			Container.BindInterfacesAndSelfTo<ScreenBlinkController>()
				.AsSingle();

			Container.Bind<VCameraResolver>()
				.AsSingle();

			Container.Bind<TargetGroupResolver>()
				.AsSingle();

			Container.BindFactory<TargetGroupAttachment.Settings, Transform, TargetGroupAttachment, TargetGroupAttachment.Factory>()
				.FromMonoPoolableMemoryPool( pool => pool
					.WithInitialSize( _cameraSettings.AttachmentPoolSize )
					.FromSubContainerResolve()
					.ByNewGameObjectMethod( subContainer => subContainer
						.Bind<TargetGroupAttachment>()
						.FromNewComponentOnRoot()
						.AsSingle()
					)
					.WithGameObjectName( "TargetGroupAttachment" )
					.UnderTransform( context => context.Container.ResolveId<Transform>( _cameraSettings.AttachmentContainerId ) )
				);

			Container.BindInterfacesAndSelfTo<CameraController>()
				.AsSingle();
		}

		private void BindTreasure()
		{
			foreach ( var beaconFactory in _beaconSettings.Factories )
			{
				Container.Bind<BeaconActor.Factory>()
					.AsCached()
					.WithArguments( beaconFactory.Prefab, beaconFactory.ResourceType );
			}

			Container.Bind<BeaconFactoryBus>()
				.AsSingle();
		}

		private void BindAudio()
		{
			Container.Bind<AudioBankLoader>()
				.AsSingle()
				.WithArguments( _audioSettings.Banks );

			Container.BindInterfacesAndSelfTo<MusicPlayer>()
				.AsSingle()
				.WithArguments( _audioSettings.Music );
		}

		private void BindFxPool()
		{
			Container.Bind<FxFactoryBus>()
				.AsSingle();
		}

		private void DeclareSignals()
		{
			// Treasure ...
			foreach ( var resource in _resourceTypes )
			{
				Container.DeclareSignal<ResourceAmountChangedSignal>()
					.WithId( resource )
					.OptionalSubscriber();

				Container.DeclareSignal<BeaconCreationStateChangedSignal>()
					.WithId( resource )
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
			Container.DeclareSignal<ToggleInventorySignal>()
				.OptionalSubscriber();

			// Guns ...
			Container.DeclareSignal<FireRateStateSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<AmmoStateSignal>()
				.OptionalSubscriber();
			Container.DeclareSignal<ReloadStateSignal>()
				.OptionalSubscriber();

			// Starting level sequence ...
			Container.DeclareSignal<StartingAreaCleansedSignal>()
				.OptionalSubscriber();
		}

		[System.Serializable]
		public class Beacon
		{
			[TableList( AlwaysExpanded = true )]
			public BeaconFactoryBus.Settings[] Factories;
		}

		[System.Serializable]
		public class Audio
		{
			[HideLabel]
			public AudioBankLoader.Settings Banks;

			[FoldoutGroup( "Music" ), HideLabel]
			public MusicPlayer.Settings Music;
		}

		[System.Serializable]
		public class Camera
		{
			[BoxGroup( "Target Groups" )]
			public string AttachmentContainerId;
			[BoxGroup( "Target Groups" )]
			public int AttachmentPoolSize;
		}
	}
}
