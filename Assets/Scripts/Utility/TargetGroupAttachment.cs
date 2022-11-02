using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
    public class TargetGroupAttachment : MonoBehaviour
    {
		private Settings _settings;
		private CinemachineTargetGroup _targetGroup;

        [Inject]
		public void Construct( Settings settings,
			CinemachineTargetGroup targetGroup )
		{
			_settings = settings;
            _targetGroup = targetGroup;
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
			[MinValue( 0 )] public float Weight;
			[MinValue( 0 )] public float Radius;
		}
	}
}
