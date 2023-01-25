using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Assets.Scripts.Installers.Player
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Player/DangerWarning" )]
	public class DangerWarningInstaller : ScriptableObjectInstaller
	{
		[SerializeField] private LayerMask _dangerFilter = -1;

		[TitleGroup( "Reactions" )]
		[ToggleGroup( "Reactions/_useCameraGroups", "Camera Groups" )]
		[SerializeField] private bool _useCameraGroups = true;
		[ToggleGroup( "Reactions/_useCameraGroups", CollapseOthersOnExpand = false ), HideLabel]
		[SerializeField] private TargetGroupAttachment.Settings _cameraGroups;

		public override void InstallBindings()
		{
			Container.BindInstance( _dangerFilter )
				.AsSingle()
				.WhenInjectedInto<DangerWarningController>();

			/* --- */

			if ( _useCameraGroups )
			{
				Container.BindInterfacesAndSelfTo<CameraGroupDangerReaction>()
					.AsSingle()
					.WithArguments( _cameraGroups );
			}
		}
	}
}