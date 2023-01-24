using Cinemachine;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
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
		private TaskRunner _weightUpdater;

		[Inject]
		public void Construct( Settings settings,
			TargetGroupResolver targetGroupResolver )
		{
			_settings = settings;
			_targetGroup = targetGroupResolver.GetTargetGroup( settings.BindingId );

			_weightUpdater = new TaskRunner( this.GetCancellationTokenOnDestroy() );
		}

		private void OnEnable()
		{
			_targetGroup.AddMember( transform, 0, _settings.Radius );
			SetWeight( _settings.Weight, _settings.EaseDuration );
		}

		private async void OnDisable()
		{
			SetWeight( 0, _settings.EaseDuration );

			if ( _weightUpdater.IsRunning )
			{
				await UniTask.WaitWhile( () => _weightUpdater.IsRunning, cancellationToken: _weightUpdater.CancelToken )
					.SuppressCancellationThrow();
			}

			_targetGroup.RemoveMember( transform );
		}

		public void SetWeight( float weight, float duration )
		{
			if ( duration <= 0 )
			{
				UpdateWeight( weight, 0 ).Forget();
				return;
			}

			_weightUpdater.Run( () => UpdateWeight( weight, duration ) )
				.Forget();
		}

		private async UniTask UpdateWeight( float weight, float duration )
		{
			int index = _targetGroup.FindMember( transform );
			if ( index < 0 )
			{
				return;
			}

			float start = _targetGroup.m_Targets[index].weight;
			float timer = 0;

			while ( timer < duration )
			{
				timer += Time.deltaTime;

				index = _targetGroup.FindMember( transform );
				_targetGroup.m_Targets[index].weight = Mathf.Lerp( start, weight, timer / duration );

				await UniTask.Yield( PlayerLoopTiming.Update );
			}

			_targetGroup.m_Targets[index].weight = weight;
		}

		[System.Serializable]
		public struct Settings
		{
			public string BindingId;

			[MinValue( 0 ), BoxGroup] 
			public float Weight;
			[MinValue( 0 ), BoxGroup] 
			public float Radius;

			[MinValue( 0 ), BoxGroup]
			public float EaseDuration;
		}
	}
}
