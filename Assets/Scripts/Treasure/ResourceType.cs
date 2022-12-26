using UnityEngine;

namespace Minipede.Gameplay.Treasures
{
	[CreateAssetMenu]
	public class ResourceType : ScriptableObject
	{
		public Color Color => _color;

		[SerializeField] private Color _color = Color.white;
	}
}