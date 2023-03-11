using Minipede.Gameplay.LevelPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	public class LevelBalanceCheat : LevelBalanceController
	{
		public override int Cycle => _settings.IsOverridingCycle ? _settings.CycleOverride : _baseBalancer.Cycle;

		private readonly Settings _settings;
		private readonly LevelBalanceController _baseBalancer;

		public LevelBalanceCheat( Settings settings, 
			SignalBus signalBus,
			LevelBalanceController baseBalancer ) 
			: base( settings, signalBus )
		{
			_settings = settings;

			_baseBalancer = baseBalancer;
		}

		public override void IncrementCycle()
		{
			if ( _settings.DisableIncrementing )
			{
				Debug.LogWarning( $"<b>{nameof( LevelBalanceCheat )}</b> is enabled.\n" +
					$"No cycles will increment." );
				return;
			}

			_baseBalancer.IncrementCycle();
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