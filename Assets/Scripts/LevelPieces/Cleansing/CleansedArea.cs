using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class CleansedArea : MonoBehaviour
    {
		private Collider2D[] _colliders;
		private PollutedAreaController _pollutionController;
		private CleansedAreaAnimator _animator;
		private CancellationToken _onDestroyCancelToken;

		[Inject]
		public void Construct( Collider2D[] colliders,
			PollutedAreaController pollutionController,
			[InjectOptional] CleansedAreaAnimator animator )
		{
			_colliders = colliders;
			_pollutionController = pollutionController;

			_animator = animator;
			if ( animator != null )
			{
				_onDestroyCancelToken = this.GetCancellationTokenOnDestroy();
			}
		}

		public void Activate()
		{
			foreach ( var collider in _colliders )
			{
				collider.enabled = true;

				_pollutionController.Cleanse( new Bounds(
					center: collider.transform.position,
					size:	collider.transform.lossyScale
				) );
			}
		}

		public void PlayFillAnimation()
		{
			_animator.Play( _onDestroyCancelToken ).Forget();
		}

		public void ImmediateFillCleansedArea()
		{
			_animator.ImmediateFillCleansedArea();
		}

		public class Factory : UnityPrefabFactory<CleansedArea>
		{
			private readonly Transform _areaContainer;

			public Factory( DiContainer container )
				: base( container )
			{
				_areaContainer = container.ResolveId<Transform>( "CleansedAreaContainer" );
			}

			public override CleansedArea Create( Object prefab, IOrientation placement, IEnumerable<object> extraArgs = null )
			{
				var result = base.Create( prefab, placement, extraArgs );

				result.transform.SetParent( _areaContainer, worldPositionStays: true );

				return result;
			}
		}
	}
}
