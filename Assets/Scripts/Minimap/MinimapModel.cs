using System.Collections.Generic;

namespace Minipede.Gameplay.Minimap
{
	public class MinimapModel
	{
		public event System.Action<IMapMarker> MarkerAdded;
		public event System.Action<IMapMarker> MarkerRemoved;

		public IEnumerable<IMapMarker> Markers => _markers;

		private readonly HashSet<IMapMarker> _markers = new HashSet<IMapMarker>();

		public void AddMarker( IMapMarker marker )
		{
			if ( _markers.Add( marker ) )
			{
				MarkerAdded?.Invoke( marker );
			}
		}

		public void RemoveMarker( IMapMarker marker )
		{
			if ( _markers.Remove( marker ) )
			{
				MarkerRemoved?.Invoke( marker );
			}
		}

		public void Clear()
		{
			foreach ( var marker in _markers )
			{
				MarkerRemoved?.Invoke( marker );
			}

			_markers.Clear();
		}
	}
}