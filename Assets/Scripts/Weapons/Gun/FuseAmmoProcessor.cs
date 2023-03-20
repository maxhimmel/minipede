using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class FuseAmmoProcessor : AmmoProcessor<FuseAmmoProcessor, FuseAmmoProcessor.Settings>
	{
		private bool _isFuseLit;
		private float _timer;

		public FuseAmmoProcessor( Settings settings ) 
			: base( settings )
		{
			_timer = settings.FuseDuration;
		}

		protected override int ReduceAmmo()
		{
			if ( _isFuseLit )
			{
				_timer = Mathf.Max( 0, _timer - _settings.FireReduction );
			}
			_isFuseLit = true;

			return Mathf.CeilToInt( _timer );
		}

		protected override void ReplenishAmmo()
		{
			_isFuseLit = false;
			_timer = _settings.FuseDuration;
		}

		public override void FixedTick()
		{
			base.FixedTick();

			if ( _isFuseLit )
			{
				_timer -= Time.deltaTime;

				if ( !HasAmmo() )
				{
					_isFuseLit = false;
					OnAmmoEmptied();
				}
			}
		}

		protected override bool HasAmmo()
		{
			return _timer > 0;
		}

		[System.Serializable]
		public new class Settings : AmmoProcessor<FuseAmmoProcessor, Settings>.Settings
		{
			[MinValue( 0 )]
			public float FuseDuration;

			public float FireReduction;
		}
	}
}