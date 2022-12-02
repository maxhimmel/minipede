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
		private TreasureSorter _sorter;

		private bool _isReleasingTreasures;
		private float _nextReleaseTime;
		private float _nextCollectTime;
		private HashSet<Treasure> _haulingTreasures = new HashSet<Treasure>();
		private List<Treasure> _treasuresWithinRange = new List<Treasure>();

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
		}

		public void ReleaseAll()
		{
			foreach ( var treasure in _haulingTreasures )
			{
				treasure.StopFollowing();
			}
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
            var treasure = otherBody?.GetComponent<Treasure>();
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
			if ( _treasuresWithinRange.Count <= 0 )
			{
				return;
			}
			if ( _nextCollectTime > Time.timeSinceLevelLoad )
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

				HaulAmountChanged?.Invoke( GetHauledTreasureWeight() );
			}

			_nextCollectTime = Time.timeSinceLevelLoad + _settings.HoldCollectDelay;
		}

		private void TryReleaseHauledTreasure()
		{
			if ( !_isReleasingTreasures )
			{
				return;
			}
			if ( _haulingTreasures.Count <= 0 )
			{
				return;
			}
			if ( _nextReleaseTime > Time.timeSinceLevelLoad )
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
			HaulAmountChanged?.Invoke( GetHauledTreasureWeight() );

			_nextReleaseTime = Time.timeSinceLevelLoad + _settings.HoldReleaseDelay;
		}

		private float GetHauledTreasureWeight()
		{
			float weight = 0;
			foreach ( var treasure in _haulingTreasures )
			{
				weight += treasure.Weight;
			}

			return weight * _settings.WeightScalar;
		}

		[System.Serializable]
		public struct Settings
		{
			public float HoldCollectDelay;
			public float HoldReleaseDelay;

			[Space, PropertyRange( 0, 1 )]
			public float WeightScalar;
		}

		private class TreasureSorter : IComparer<Treasure>
		{
			private readonly Rigidbody2D _owner;

			public TreasureSorter( Rigidbody2D owner )
			{
				_owner = owner;
			}

			public int Compare( Treasure lhs, Treasure rhs )
			{
				float lhsDistSqr = (lhs.transform.position.ToVector2() - _owner.position).sqrMagnitude;
				float rhsDistSqr = (rhs.transform.position.ToVector2() - _owner.position).sqrMagnitude;

				return lhsDistSqr > rhsDistSqr
					? -1
					: 1;
			}
		}
	}
}
