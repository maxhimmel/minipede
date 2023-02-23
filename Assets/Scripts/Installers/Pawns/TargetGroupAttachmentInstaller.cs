using Minipede.Gameplay.Cameras;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class TargetGroupAttachmentInstaller : MonoInstaller
    {
        [SerializeField] private TargetGroupAttachment.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<TargetGroupAttachment>()
				.FromMethod( () => 
					Container
						.Resolve<TargetGroupAttachment.Factory>()
						.Create( _settings, Container.Resolve<Transform>() ) 
				)
				.AsCached()
				.NonLazy();
		}
	}
}
