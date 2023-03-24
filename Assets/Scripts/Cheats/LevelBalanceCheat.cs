using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	public class LevelBalanceCheat : LevelBalanceController
	{
		public override int Cycle => _settings.IsOverridingCycle ? _settings.CycleOverride : _currentCycle;

		private readonly Settings _settings;

		private int _currentCycle;

		public LevelBalanceCheat( Settings settings,
			SignalBus signalBus,
			LevelBalanceController baseBalancer )
			: base( settings, signalBus )
		{
			_settings = settings;
			_currentCycle = settings.StartCycle;
		}

		public override void IncrementCycle()
		{
			if ( _settings.DisableIncrementing )
			{
				Debug.LogWarning( $"<b>{nameof( LevelBalanceCheat )}</b> is enabled.\n" +
					$"No cycles will increment." );
				return;
			}

			_signalBus.Fire( new LevelCycleChangedSignal( ++_currentCycle ) );
			Debug.Log( $"<color=orange>Level cycle incremented:</color> (<b>{_currentCycle}</b>)" );
		}

		[System.Serializable]
		public new class Settings : LevelBalanceController.Settings
		{
			public bool DisableIncrementing;

			[HorizontalGroup, LabelText( "Override Cycle" )]
			public bool IsOverridingCycle;
			[HorizontalGroup]
			[MinValue( 0 ), EnableIf( "IsOverridingCycle" ), HideLabel]
			public int CycleOverride;
		}
	}
}