namespace Minipede.Gameplay.Vfx
{
	public interface IFxAnimator
	{
		void Play( IFxSignal signal );

		public interface Settings
		{
			System.Type AnimatorType { get; }
		}
	}
}