using System;
using UnityEngine;

namespace Minipede.Utility
{
	[System.Serializable]
	public class WeightedNode<T> : WeightedNode
	{
		public T Item;
	}

	[System.Serializable]
	public class WeightedNode
	{
		public int NormalizedWeight { get { return _normalizedWeight; } }

		[Range( WeightedList.MinWeight, WeightedList.MaxWeight )]
		[SerializeField] private int _normalizedWeight = 0;

		[NonSerialized] public int Weight = 0;
	}
}
