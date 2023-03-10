using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/Mushroom Balance Table" )]
	public class MushroomBalances : ScriptableObject
	{
		public CurveEvaluator Health => _health;

		[SerializeField] private CurveEvaluator _health;
	}
}