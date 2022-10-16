using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
	public class SineMotorInstaller :
		MotorInstaller<SineMotor, SineMotor.Settings>
	{
		[Inject] private DragonflyController.Settings _dragonfly;

		public override SineMotor.Settings GetMotorSettings()
		{
			var combinedSettings = _settings;
			combinedSettings.Wave.Amplitude = _dragonfly.ZigZagRange.Random( false );

			Debug.Log( combinedSettings.Wave.Amplitude );
			return combinedSettings;
		}
	}
}
