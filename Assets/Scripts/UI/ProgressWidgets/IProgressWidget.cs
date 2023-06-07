namespace Minipede.Gameplay.UI
{
	public interface IProgressWidget
	{
		float NormalizedProgress { get; }

		void SetProgress( float normalizedProgress );
	}
}