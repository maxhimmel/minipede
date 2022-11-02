
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Minipede.Utility
{
	[System.Serializable]
	public class WeightedList
	{
		public const int MinWeight = 0;
		public const int MaxWeight = 100;
	}

	[System.Serializable]
	public class WeightedList<T, K> : WeightedList
		where T : WeightedNode<K>
	{
		public IEnumerable<K> Items => _items.Select( node => node.Item );

		[Tooltip( "If a '0' is rolled this will return nothing." )]
		[SerializeField] private bool _allowEmptyRolls = true;
		[SerializeField] private T[] _items = default;

		private int _maxWeight = 0;

		public void Init()
		{
			if ( _items == null ) { return; }

			int weightSum = 0;
			foreach ( T node in _items )
			{
				weightSum += node.NormalizedWeight;
				node.Weight = weightSum;
			}

			_maxWeight = weightSum;
		}

		public K GetRandomItem()
		{
			if ( _maxWeight <= 0 )
			{
				Debug.LogWarning( $"{typeof( T ).Name} | Attempting to get random item without being initialized." );
				return default;
			}

			int roll = Random.Range( 0, _maxWeight + 1 );
			if ( _allowEmptyRolls && roll <= 0 ) { return default; }

			foreach ( T node in _items )
			{
				if ( node.Weight > 0 && roll <= node.Weight )
				{
					return node.Item;
				}
			}

			return default;
		}
	}
}