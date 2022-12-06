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
			Container.Bind<MaxSpeedScalar>()
				.FromResolveGetter( ( DiContainer c ) => c.ResolveId<MaxSpeedScalar>( _id ) )
				.AsSingle();

			Container.Decorate<IMaxSpeed>()
				.With<MaxSpeedDecorator>();
		}
	}
}

namespace Minipede.Gameplay.Movement
{
	public class MaxSpeedDecorator : IMaxSpeed
	{
		private readonly IMaxSpeed _maxSpeed;
		private readonly MaxSpeedScalar _speedScalar;

		public MaxSpeedDecorator( IMaxSpeed maxSpeedSettings,
			MaxSpeedScalar speedScalar )
		{
			_maxSpeed = maxSpeedSettings;
			_speedScalar = speedScalar;
		}

		public float GetMaxSpeed()
		{
			return _maxSpeed.GetMaxSpeed() * _speedScalar.Scale;
		}

		public void RestoreMaxSpeed()
		{
			_maxSpeed.RestoreMaxSpeed();
		}

		public void SetMaxSpeed( float maxSpeed )
		{
			// No need to apply scaling here. We apply it when GetMaxSpeed is called.
			_maxSpeed.SetMaxSpeed( maxSpeed );
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