using Minipede.Gameplay.UI;
using UnityEngine;

namespace Minipede.Gameplay.Minimap
{
	public interface IMapMarker
	{
		Transform Avatar { get; }
		MinimapMarker MarkerPrefab { get; }
	}
}