using UnityEngine;

namespace Minipede.Gameplay.Vfx
{
	public interface IVfxAnimator
	{
		void Play( IVfxSignal signal );

		public interface Settings
		{
			System.Type AnimatorType { get; }
		}
	}
}