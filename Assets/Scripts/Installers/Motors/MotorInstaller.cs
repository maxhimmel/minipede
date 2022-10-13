using Minipede.Gameplay.Movement;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class MotorInstaller<TMotor, TSettings> : ScriptableObjectInstaller
		where TMotor : IMotor
	{
		[SerializeField] private TSettings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TMotor>()
				.AsTransient()
				.WithArguments( _settings );
		}
	}
}
