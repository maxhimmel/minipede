using Minipede.Gameplay.UI;
using UnityEngine;

namespace Minipede.Gameplay.Minimap
{
	[System.Serializable]
	public class MapMarker : IMapMarker
	{
		Transform IMapMarker.Avatar => Avatar;
		MinimapMarker IMapMarker.MarkerPrefab => MarkerPrefab;

		public Transform Avatar;
		public MinimapMarker MarkerPrefab;
	}
}