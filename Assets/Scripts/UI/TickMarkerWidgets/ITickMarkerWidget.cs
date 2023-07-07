namespace Minipede.Gameplay.UI
{
	public interface ITickMarkerWidget
    {
        void PlaceTickMarkers( int tickCount );
        void PlaceTickMarker( float progress );
    }
}
