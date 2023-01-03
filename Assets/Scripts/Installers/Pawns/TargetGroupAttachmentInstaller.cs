using Minipede.Gameplay.Cameras;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/TargetGroupAttachment" )]
    public class TargetGroupAttachmentInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private TargetGroupAttachment.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<TargetGroupAttachment>()
				.FromNewComponentOnRoot()
				.AsCached()
				.WithArguments( _settings )
				.NonLazy();
		}
	}
}
