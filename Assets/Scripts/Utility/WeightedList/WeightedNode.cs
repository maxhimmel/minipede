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

		[HideInInspector] public int Weight = 0;
	}
}
