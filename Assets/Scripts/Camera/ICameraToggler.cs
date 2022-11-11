namespace Minipede.Gameplay.Camera
{
	public interface ICameraToggler<TToggler>
	{
		void Activate( TToggler sender );
	}
}