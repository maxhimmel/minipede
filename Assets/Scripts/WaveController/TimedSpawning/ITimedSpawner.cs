using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public interface ITimedSpawner : ITickable,
		IInitializable,
		IDisposable
	{
		void Play();
		void Stop();

		UniTaskVoid HandleSpawning( CancellationToken cancelToken );

		[HideReferenceObjectPicker]
		public interface ISettings
		{
			System.Type SpawnerType { get; }
			string Name { get; }
		}
	}
}