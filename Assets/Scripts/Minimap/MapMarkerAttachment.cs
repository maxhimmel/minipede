using Minipede.Gameplay.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Minimap
{
	public class MapMarkerAttachment : MonoBehaviour,
		IMapMarker
	{
		public Transform Avatar => _settings.Avatar;
		public MinimapMarker MarkerPrefab => _settings.MarkerPrefab;

		[HideLabel]
		[SerializeField] private MapMarker _settings;
	}
}