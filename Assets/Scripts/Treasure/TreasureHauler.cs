using System.Collections.Generic;
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
		private TreasureSorter _sorter;
		private HashSet<IFollower> _haulingTreasures;
		private List<IFollower> _treasuresWithinRange;

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

            haulTrigger.enabled = false;

			_sorter = new TreasureSorter( body );

			_haulingTreasures = new HashSet<IFollower>();
			_treasuresWithinRange = new List<IFollower>();
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
			_haulWeight = 0;
			_haulingTreasures.Clear();
			HaulAmountChanged?.Invoke( 0 );
		}

		public void StartReleasingTreasure()
		{
			_isReleasingTreasures = true;

			StopGrabbing();
		}

		public void StopReleasingTreasure()
		{
			_isReleasingTreasures = false;
			_nextReleaseTime = 0;
		}

        public void StartGrabbing()
		{
            _haulTrigger.enabled = true;

			StopReleasingTreasure();
		}

        public void StopGrabbing()
		{
            _haulTrigger.enabled = false;
			_treasuresWithinRange.Clear();
			_nextCollectTime = 0;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
            var otherBody = collision.attachedRigidbody;
            var treasure = otherBody?.GetComponent<IFollower>();
            if ( treasure != null )
			{
                _treasuresWithinRange.Add( treasure );
			}
		}

		private void Update()
		{
			TryHaulTreasuresWithinRange();
			TryReleaseHauledTreasure();
		}

		private void TryHaulTreasuresWithinRange()
		{
			if ( !CanHaulTreasure() )
			{
				return;
			}

			_treasuresWithinRange.Sort( _sorter );
			int lastIndex = _treasuresWithinRange.Count - 1;
			var closestTreasure = _treasuresWithinRange[lastIndex];

			if ( _haulingTreasures.Add( closestTreasure ) )
			{
				_treasuresWithinRange.RemoveAt( lastIndex );
				closestTreasure.Follow( _body );

				_haulWeight += closestTreasure.Weight;
				HaulAmountChanged?.Invoke( GetHauledTreasureWeight() );
			}

			_nextCollectTime = Time.timeSinceLevelLoad + _settings.HoldCollectDelay;
		}

		private bool CanHaulTreasure()
		{
			return _treasuresWithinRange.Count > 0
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

			_haulWeight = Mathf.Max( 0, _haulWeight - treasure.Weight );
			_haulingTreasures.Remove( treasure );
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

		[System.Serializable]
		public struct Settings
		{
			public float HoldCollectDelay;
			public float HoldReleaseDelay;

			[Space, PropertyRange( 0, 1 )]
			public float WeightScalar;
		}

		private class TreasureSorter : IComparer<IFollower>
		{
			private readonly Rigidbody2D _owner;

			public TreasureSorter( Rigidbody2D owner )
			{
				_owner = owner;
			}

			public int Compare( IFollower lhs, IFollower rhs )
			{
				float lhsDistSqr = (lhs.Body.position - _owner.position).sqrMagnitude;
				float rhsDistSqr = (rhs.Body.position - _owner.position).sqrMagnitude;

				return lhsDistSqr > rhsDistSqr
					? -1
					: 1;
			}
		}
	}
}
