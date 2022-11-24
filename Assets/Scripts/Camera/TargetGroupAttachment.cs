using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Cameras
{
    public class TargetGroupAttachment : MonoBehaviour
    {
		public string Id => _settings.BindingId;

		private Settings _settings;
		private CinemachineTargetGroup _targetGroup;

        [Inject]
		public void Construct( Settings settings,
			TargetGroupResolver targetGroupResolver )
		{
			_settings = settings;
			_targetGroup = targetGroupResolver.GetTargetGroup( settings.BindingId );
		}

		private void OnEnable()
		{
			_targetGroup.AddMember( transform, _settings.Weight, _settings.Radius );
		}

		private void OnDisable()
		{
			_targetGroup.RemoveMember( transform );
		}

		[System.Serializable]
		public struct Settings
		{
			public string BindingId;

			[MinValue( 0 ), BoxGroup] 
			public float Weight;
			[MinValue( 0 ), BoxGroup] 
			public float Radius;
		}
	}
}
