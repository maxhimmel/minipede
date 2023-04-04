using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public interface IMinimap
	{
		void AddMarker( Transform avatar, MinimapMarker markerPrefab );
		void RemoveMarker( Transform avatar );
	}
}