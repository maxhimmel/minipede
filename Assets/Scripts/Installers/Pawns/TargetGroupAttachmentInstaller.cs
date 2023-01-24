using Minipede.Gameplay.Cameras;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/TargetGroupAttachment" )]
    public class TargetGroupAttachmentInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private bool _enableOnStart = true;
        [SerializeField] private TargetGroupAttachment.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<TargetGroupAttachment>()
				.FromNewComponentOnRoot()
				.AsCached()
				.WithArguments( _settings )
				.OnInstantiated( (context, obj) =>
				{
					if ( obj is TargetGroupAttachment attachment )
					{
						attachment.enabled = _enableOnStart;
					}
				} )
				.NonLazy();
		}
	}
}
