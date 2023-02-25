using System;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public interface ITimedSpawner : ITickable,
		IInitializable,
		IDisposable
	{
		void Play();
		void Stop();
	}
}