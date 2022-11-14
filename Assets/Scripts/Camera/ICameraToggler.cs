namespace Minipede.Gameplay.Cameras
{
	public interface ICameraToggler<TToggler>
	{
		void Activate( TToggler sender );
		void Deactivate( TToggler sender );
	}
}