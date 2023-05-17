using System.Collections.Generic;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Treasures
{
	public class TreasureHauler : MonoBehaviour
	{
		public event TreaseHaulSignature HaulAmountChanged;
		public delegate void TreaseHaulSignature( float weight );

		private Settings _settings;
		private Rigidbody2D _body;
		private Collider2D _haulTrigger;
		private HashSet<Haulable> _haulingTreasures;
		private List<Haulable> _treasuresWithinRange;

		private Haulable _selectedHaulable;
		private bool _isHaulingRequested;
		private float _haulWeight;
		private bool _isReleasingTreasures;
		private float _nextReleaseTime;
		private float _nextCollectTime;

		[Inject]
		public void Construct( Settings settings,
			Rigidbody2D body,
			Collider2D haulTrigger )
		{
			_settings = settings;
			_body = body;
			_haulTrigger = haulTrigger;

			_haulingTreasures = new HashSet<Haulable>();
			_treasuresWithinRange = new List<Haulable>();
		}

		public void CollectAll( Rigidbody2D collector )
		{
			foreach ( var treasure in _haulingTreasures )
			{
				treasure.SnapToCollector( collector );
			}
			ClearHaul();
		}

		public void ReleaseAll()
		{
			foreach ( var treasure in _haulingTreasures )
			{
				treasure.StopFollowing();
			}
			ClearHaul();
		}

		private void ClearHaul()
		{
			if ( _selectedHaulable != null )
			{
				_selectedHaulable.Deselect();
				_selectedHaulable = null;
			}

			_haulingTreasures.Clear();

			_haulWeight = 0;
			HaulAmountChanged?.Invoke( 0 );
		}

		public void ReleaseTreasure( Haulable haulable )
		{
			if ( _haulingTreasures.Remove( haulable ) )
			{
				_treasuresWithinRange.Remove( haulable );

				_haulWeight = Mathf.Max( 0, _haulWeight - haulable.Weight );
				HaulAmountChanged?.Invoke( GetHauledTreasureWeight() );
			}
		}

		public void StartReleasingTreasure()
		{
			_isReleasingTreasures = true;

			StopGrabbing();
		}

		public void StopGrabbing()
		{
			_isHaulingRequested = false;
			_nextCollectTime = 0;
		}

		public void StartGrabbing()
		{
			_isHaulingRequested = true;

			StopReleasingTreasure();
		}

		public void StopReleasingTreasure()
		{
			_isReleasingTreasures = false;
			_nextReleaseTime = 0;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			if ( collision.TryGetComponentFromBody<Haulable>( out var treasure ) )
			{
				if ( !_haulingTreasures.Contains( treasure ) )
				{
					_treasuresWithinRange.Add( treasure );
				}
			}
		}

		private void OnTriggerExit2D( Collider2D collision )
		{
			if ( collision.TryGetComponentFromBody<Haulable>( out var treasure ) )
			{
				_treasuresWithinRange.Remove( treasure );
			}
		}

		private void Update()
		{
			TryHaulTreasuresWithinRange();
			TryReleaseHauledTreasure();

			UpdateSelectedHaulable();
		}

		private void TryHaulTreasuresWithinRange()
		{
			if ( !CanHaulTreasure() )
			{
				return;
			}

			// Sort from farthest(0) --> closest(N) ...
			_treasuresWithinRange.Sort( ( lhs, rhs ) =>
			{
				float lhsDistSqr = (lhs.Body.position - _body.position).sqrMagnitude;
				float rhsDistSqr = (rhs.Body.position - _body.position).sqrMagnitude;

				return lhsDistSqr > rhsDistSqr
					? -1
					: 1;
			} );

			int lastIndex = _treasuresWithinRange.Count - 1;
			var closestTreasure = _treasuresWithinRange[lastIndex];

			if ( _haulingTreasures.Add( closestTreasure ) )
			{
				_treasuresWithinRange.RemoveAt( lastIndex );
				closestTreasure.Follow( _body );
				closestTreasure.SetHauler( this );

				_haulWeight += closestTreasure.Weight;
				HaulAmountChanged?.Invoke( GetHauledTreasureWeight() );

				_nextCollectTime = Time.timeSinceLevelLoad + _settings.HoldCollectDelay;
			}
		}

		private bool CanHaulTreasure()
		{
			return _isHaulingRequested
				&& _treasuresWithinRange.Count > 0
				&& _nextCollectTime <= Time.timeSinceLevelLoad;
		}

		private void TryReleaseHauledTreasure()
		{
			if ( !CanReleaseTreasure() )
			{
				return;
			}

			var enumerator = _haulingTreasures.GetEnumerator();
			if ( !enumerator.MoveNext() )
			{
				return;
			}

			var treasure = enumerator.Current;
			treasure.StopFollowing();

			_haulingTreasures.Remove( treasure );

			if ( _haulTrigger.IsTouching( treasure.Collider ) )
			{
				_treasuresWithinRange.Add( treasure );
			}

			_haulWeight = Mathf.Max( 0, _haulWeight - treasure.Weight );
			HaulAmountChanged?.Invoke( GetHauledTreasureWeight() );

			_nextReleaseTime = Time.timeSinceLevelLoad + _settings.HoldReleaseDelay;
		}

		private bool CanReleaseTreasure()
		{
			return _isReleasingTreasures
				&& _haulingTreasures.Count > 0
				&& _nextReleaseTime <= Time.timeSinceLevelLoad;
		}

		public float GetHauledTreasureWeight()
		{
			return _haulWeight * _settings.WeightScalar;
		}

		private void UpdateSelectedHaulable()
		{
			if ( _treasuresWithinRange.Count <= 0 )
			{
				if ( _selectedHaulable != null )
				{
					_selectedHaulable.Deselect();
					_selectedHaulable = null;
				}

				return;
			}

			Haulable closestHaulable = null;
			float closestDistSqr = Mathf.Infinity;

			foreach ( var haulable in _treasuresWithinRange )
			{
				float distSqr = (haulable.Body.position - _body.position).sqrMagnitude;
				if ( distSqr < closestDistSqr )
				{
					closestDistSqr = distSqr;
					closestHaulable = haulable;
				}
			}

			if ( _selectedHaulable != closestHaulable )
			{
				if ( _selectedHaulable != null )
				{
					_selectedHaulable.Deselect();
				}

				_selectedHaulable = closestHaulable;
				_selectedHaulable.Select();
			}
		}

		public bool TryGetFirst<THaulable>( out THaulable result )
			where THaulable : Haulable
		{
			foreach ( var haulable in _haulingTreasures )
			{
				if ( haulable is THaulable first )
				{
					result = first;
					return true;
				}
			}

			result = null;
			return false;
		}

		public bool IsHaulingType<THaulable>()
			where THaulable : Haulable
		{
			foreach ( var haulable in _haulingTreasures )
			{
				if ( haulable is THaulable )
				{
					return true;
				}
			}

			return false;
		}

		[System.Serializable]
		public class Settings
		{
			public float HoldCollectDelay;
			public float HoldReleaseDelay;

			[Space, PropertyRange( 0, 1 )]
			public float WeightScalar;
		}
	}
}