namespace Minipede.Gameplay.Fx
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