namespace Minipede.Gameplay.Fx
{
	public interface IFxAnimator
	{
		void Play( IFxSignal signal );

		public interface ISettings
		{
			System.Type AnimatorType { get; }
		}
	}
}