using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class EndGameController : IDisposable
	{
		private readonly Settings _settings;
		private readonly SignalBus _signalBus;
		private readonly ShipController _shipController;
		private readonly SceneLoader _sceneLoader;

		private bool _canWin;

		public EndGameController( Settings settings,
			SignalBus signalBus,
			ShipController shipController,
			SceneLoader sceneLoader )
		{
			_settings = settings;
			_signalBus = signalBus;
			_shipController = shipController;
			_sceneLoader = sceneLoader;

			signalBus.Subscribe<IWinStateChangedSignal>( OnWinStateChanged );
			shipController.Possessed += OnShipPossessed;
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<IWinStateChangedSignal>( OnWinStateChanged );
			_shipController.Possessed -= OnShipPossessed;
		}

		private void OnWinStateChanged( IWinStateChangedSignal signal )
		{
			_canWin = signal.CanWin;

			if ( signal.CanWin )
			{
				// Design idea: FINAL HORDE
					// Start spawning a crazy assortment of never ending enemies!
					// Also start counting down the remainder of the pollution area.
					// Once the pollution area has reached zero THEN we launch the end-game sequence.
			}
		}

		private void OnShipPossessed( Ship ship )
		{
			if ( !_canWin )
			{
				return;
			}

			_shipController.EnterEvacuationMode();

			PlayEndSequence( ship )
				.Cancellable( AppHelper.AppQuittingToken )
				.Forget();
		}

		private async UniTask PlayEndSequence( Ship ship )
		{
			await StartFlyAwaySequence( ship );
			await StartTakeoffSequence( ship );

			_sceneLoader.Load( _settings.NextLevelName ).Forget();

			// Unload scene
			// Load transition scene where ship looks like it's flying over scrolling mushrooms
			// Fade in
			// Wait a couple seconds ???
			// Fade out 
			// Reload the main gameplay scene

			// Get all lighthouses
				// Blink each lighthouse once
				// For N second(s) ...
					// Start blinking each lighthouse in unison at ever-increasing rate
						// Also blink ship in unison with lighthouses
				//After N seconds(s) ...
					// Ship starts lerping between lighthouses at ever-increasing rate
					// Lighthouse cleanse areas expand to fil remainder of polluted areas
				// Once polluted areas are fully cleansed ...
					// Ship takes off towards top-center of screen
		}

		private async UniTask StartFlyAwaySequence( Ship ship )
		{
			await TaskHelpers.DelaySeconds( _settings.TakeoffDelay );

			Vector2 startPos = ship.Body.position;
			Vector2 endPos = _settings.CenteringPosition;

			float timer = 0;
			while ( timer < _settings.CenteringDuration )
			{
				timer += Time.deltaTime;
				timer = Mathf.Min( timer, _settings.CenteringDuration );

				float animValue = Tweens.Ease( _settings.CenteringAnim, timer, _settings.CenteringDuration );
				Vector2 newPos = Vector2.LerpUnclamped( startPos, endPos, animValue );

				ship.Body.MovePosition( newPos );

				await UniTask.Yield( PlayerLoopTiming.FixedUpdate, AppHelper.AppQuittingToken );
			}
		}

		private async UniTask StartTakeoffSequence( Ship ship )
		{
			await TaskHelpers.DelaySeconds( _settings.TakeoffDelay );

			Vector2 startPos = ship.Body.position;
			Vector2 endPos = _settings.TakeoffDestination;

			float timer = 0;
			while ( timer < _settings.TakeoffDuration )
			{
				timer += Time.deltaTime;
				timer = Mathf.Min( timer, _settings.TakeoffDuration );

				float animValue = Tweens.Ease( _settings.TakeoffAnim, timer, _settings.TakeoffDuration );
				Vector2 newPos = Vector2.LerpUnclamped( startPos, endPos, animValue );

				ship.Body.MovePosition( newPos );

				await UniTask.Yield( PlayerLoopTiming.FixedUpdate, AppHelper.AppQuittingToken );
			}
		}

		[System.Serializable]
		public class Settings
		{
			public string NextLevelName = "Scene 0-0";

			[BoxGroup( "Centering" )]
			public float CenteringDelay;
			[BoxGroup( "Centering" )]
			public float CenteringDuration;
			[BoxGroup( "Centering" )]
			public Tweens.Function CenteringAnim;
			[BoxGroup( "Centering" )]
			public Vector2 CenteringPosition;

			[BoxGroup( "Takeoff" )]
			public float TakeoffDelay;
			[BoxGroup( "Takeoff" )]
			public float TakeoffDuration;
			[BoxGroup( "Takeoff" )]
			public Tweens.Function TakeoffAnim;
			[BoxGroup( "Takeoff" )]
			public Vector2 TakeoffDestination;
		}
	}
}