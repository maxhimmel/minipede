using Minipede.Gameplay;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;

namespace Minipede.Installers
{
	public class BeaconSettings : HaulableSettings
	{
		[BoxGroup( "Beacon" )]
		[SerializeField] private PositionConstraint _equipConstraint;
		[BoxGroup( "Beacon" )]
		[SerializeField] private ParticleSystem _cleansingPreviewVfx;

		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.Bind<IPushable>()
				.FromMethod( GetComponent<IPushable> )
				.AsSingle();

			Container.BindInstance( _equipConstraint );
			Container.BindInstance( _cleansingPreviewVfx );
		}
	}
}