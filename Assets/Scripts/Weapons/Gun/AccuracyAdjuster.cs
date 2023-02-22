using System;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class AccuracyAdjuster : AngleDirectionAdjuster,
		IFireEndProcessor,
		IFixedTickable
	{
		private readonly Settings _settings;

		private float _currentRecoil;
		private float _endFireRateTime;

		public AccuracyAdjuster( Settings settings ) 
			: base( settings.Base )
		{
			_settings = settings;
		}

		public void FireEnding()
		{
			_currentRecoil = Mathf.MoveTowards( _currentRecoil, 1, 1f / _settings.ShotsToFullRecoil );
			_endFireRateTime = Time.timeSinceLevelLoad + _settings.FireRate;
		}

		public void FixedTick()
		{
			if ( _endFireRateTime > Time.timeSinceLevelLoad )
			{
				return;
			}

			_currentRecoil = Mathf.MoveTowards( _currentRecoil, 0, Time.fixedDeltaTime / _settings.RebalanceDuration );
		}

		protected override float GetAngle()
		{
			float baseAngle = base.GetAngle();

			float recoilScale = Mathf.Clamp01( _currentRecoil );
			if ( Mathf.Approximately( recoilScale, 0 ) )
			{
				return baseAngle;
			}

			float recoilAngle = _settings.MaxAngleRange.Random() * recoilScale;
			return baseAngle + recoilAngle;
		}

		[System.Serializable]
		public new struct Settings : IGunModule
		{
			public Type ModuleType => typeof( AccuracyAdjuster );

			[HideLabel]
			public AngleDirectionAdjuster.Settings Base;

			[MinMaxSlider( 0, 180f, ShowFields = true )]
			public Vector2 MaxAngleRange;

			[BoxGroup( "Recoil" ), MinValue( 1 )]
			public int ShotsToFullRecoil;
			[BoxGroup( "Recoil" )]
			public float RebalanceDuration;
			[BoxGroup( "Recoil" )]
			public float FireRate;
		}
	}
}