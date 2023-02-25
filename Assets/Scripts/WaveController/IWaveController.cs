using Cysharp.Threading.Tasks;

namespace Minipede.Gameplay.Waves
{
	public interface IWaveController
	{
		UniTask Play();
		void Interrupt();
	}
}