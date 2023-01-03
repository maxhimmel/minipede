using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Motors/" + nameof( SineMotor ) )]
	public class SineMotorInstaller :
		ConfigurableInstaller<SineMotor, SineMotor.Settings>
	{
		[Inject] private DragonflyController.Settings _dragonfly;

		public override SineMotor.Settings GetSettings()
		{
			var combinedSettings = _settings;
			combinedSettings.Wave.Amplitude = _dragonfly.ZigZagRange.Random( false );

			return combinedSettings;
		}
	}
}
