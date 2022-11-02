using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede
{
    [CreateAssetMenu( menuName = "Target Group Attachment" )]
    public class TargetGroupAttachmentInstaller : ScriptableObjectInstaller
    {
        [SerializeField] private TargetGroupAttachment.Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<TargetGroupAttachment>()
				.FromNewComponentOnRoot()
				.AsSingle()
				.WithArguments( _settings )
				.NonLazy();
		}
	}
}
