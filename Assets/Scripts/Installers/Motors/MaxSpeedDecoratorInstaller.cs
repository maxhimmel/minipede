using Minipede.Gameplay.Movement;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = "Motors/Decorators/Max Speed" )]
    public class MaxSpeedDecoratorInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private string _id;

		public override void InstallBindings()
		{
			//Container.BindInterfacesAndSelfTo<MaxSpeedScalar>()
			//	.FromResolve( "EnemySpeedScalar" )
			//	.AsSingle();
			Container.Bind<MaxSpeedScalar>()
				.FromResolveGetter( ( DiContainer c ) => c.ResolveId<MaxSpeedScalar>( "EnemySpeedScalar" ) )
				.AsSingle();

			Container.Decorate<IMotor.ISettings>()
				.With<MaxSpeedDecorator>();
		}
	}
}

namespace Minipede.Gameplay.Movement
{
	public class MaxSpeedDecorator : IMotor.ISettings
	{
		public float MaxSpeed => _settings.MaxSpeed * _speedScalar.Scale;

		private readonly IMotor.ISettings _settings;
		private readonly MaxSpeedScalar _speedScalar;

		public MaxSpeedDecorator( IMotor.ISettings settings,
			MaxSpeedScalar speedScalar )
		{
			_settings = settings;
			_speedScalar = speedScalar;
		}
	}

	public class MaxSpeedScalar
	{
		public float Scale { get; private set; } = 1;

		public void SetScale( float newScale )
		{
			Scale = newScale;
		}

		public void Tick()
		{
			if ( Input.GetKeyDown( KeyCode.UpArrow ) )
			{
				Scale += 0.25f;
				Debug.Log( $"Up! : {Scale}" );
			}
			if ( Input.GetKeyDown( KeyCode.DownArrow ) )
			{
				Scale -= 0.25f;
				Debug.Log( $"Down! : {Scale}" );
			}
		}
	}
}