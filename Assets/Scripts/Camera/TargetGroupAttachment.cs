using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Cameras
{
    public class TargetGroupAttachment : MonoBehaviour,
		IPoolable<TargetGroupAttachment.Settings, Transform, IMemoryPool>,
		IDisposable
    {
		public string Id => _settings.BindingId;
		public bool IsActive { get; private set; }

		private TargetGroupResolver _targetGroupResolver;
		private TaskRunner _weightUpdater;
		private Settings _settings;
		private CinemachineTargetGroup _targetGroup;
		private IMemoryPool _pool;

		[Inject]
		public void Construct( TargetGroupResolver targetGroupResolver )
		{
			_targetGroupResolver = targetGroupResolver;
			_weightUpdater = new TaskRunner( this.GetCancellationTokenOnDestroy() );
		}

		public void Dispose()
		{
			_pool.Despawn( this );
		}

		public void OnDespawned()
		{
			_pool = null;
			_targetGroup = null;
		}

		public void OnSpawned( Settings settings, Transform parent, IMemoryPool pool )
		{
			_settings = settings;
			_targetGroup = _targetGroupResolver.GetTargetGroup( settings.BindingId );
			_pool = pool;

			transform.SetParent( parent, false );

			Activate();
		}

		public void Activate()
		{
			if ( IsActive )
			{
				throw new System.NotImplementedException( $"{nameof(TargetGroupAttachment)} has already been activated." );
			}

			IsActive = true;

			_targetGroup.AddMember( transform, 0, _settings.Radius );
			SetWeight( _settings.Weight, _settings.EaseDuration );
		}

		public async void Deactivate( bool canDispose )
		{
			SetWeight( 0, _settings.EaseDuration );

			if ( _weightUpdater.IsRunning )
			{
				await UniTask.WaitWhile( () => _weightUpdater.IsRunning, cancellationToken: _weightUpdater.CancelToken )
					.SuppressCancellationThrow();
			}

			_targetGroup.RemoveMember( transform );

			IsActive = false;

			if ( canDispose )
			{
				Dispose();
			}
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

		public class Factory : PlaceholderFactory<Settings, Transform, TargetGroupAttachment> { }
	}
}
