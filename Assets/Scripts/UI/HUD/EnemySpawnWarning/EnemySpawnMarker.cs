using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Fx;
using Minipede.Utility;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class EnemySpawnMarker : MonoBehaviour,
        IPoolable<IOrientation, IMemoryPool>,
        IDisposable
	{
		[Header( "Elements" )]
		[SerializeField] private CanvasGroup _fader;
		[SerializeField] private TMP_Text _count;

		[Header( "Animation" )]
		[SerializeField] private float _diposeAnimationDuration = 0.25f;
		[SerializeField] private BounceScaler.Settings _countBounce;


		private SignalBus _signalBus;

		private IMemoryPool _pool;
		private CancellationTokenSource _onDestroyCancelSource;
		private BounceScaler _textBouncer;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		private void Awake()
		{
			_onDestroyCancelSource = AppHelper.CreateLinkedCTS( this.GetCancellationTokenOnDestroy() );

			_textBouncer = new BounceScaler( _count.transform );
		}

		public void Dispose()
		{
			AnimatedDispose().Forget();
		}

		private async UniTaskVoid AnimatedDispose()
		{
			_signalBus.TryFireId( "Dispose", new FxSignal(
				transform.position,
				Vector2.up,
				transform
			) );

			await TaskHelpers.DelaySeconds( _diposeAnimationDuration, _onDestroyCancelSource.Token );

			_pool?.Despawn( this );
		}

		public void OnDespawned()
		{
			_pool = null;
		}

		public void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			_pool = pool;

			transform.position = placement.Position;
			transform.rotation = placement.Rotation;

			_signalBus.TryFireId( "Spawn", new FxSignal(
				transform.position,
				Vector2.up,
				transform
			) );
		}

		public void SetCount( int count )
		{
            _count.text = count.ToString();

			_textBouncer.Bounce( _countBounce )
				.Cancellable( _onDestroyCancelSource.Token );
		}
    }
}
