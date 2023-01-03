using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelMushroomHealer : IInitializable,
		IDisposable
	{
		private readonly SignalBus _signalBus;
		private readonly List<Mushroom> _mushrooms;

		public LevelMushroomHealer( SignalBus signalBus )
		{
			_signalBus = signalBus;

			_mushrooms = new List<Mushroom>();
		}

		public void Initialize()
		{
			_signalBus.Subscribe<BlockSpawnedSignal>( OnBlockSpawned );
			_signalBus.Subscribe<BlockDestroyedSignal>( OnBlockDestroyed );
		}

		public void Dispose()
		{
			_signalBus.TryUnsubscribe<BlockSpawnedSignal>( OnBlockSpawned );
			_signalBus.TryUnsubscribe<BlockDestroyedSignal>( OnBlockDestroyed );
		}

		private void OnBlockSpawned( BlockSpawnedSignal signal )
		{
			if ( signal.NewBlock is Mushroom newMushroom )
			{
				_mushrooms.Add( newMushroom );
			}
		}

		private void OnBlockDestroyed( BlockDestroyedSignal signal )
		{
			if ( signal.Victim is Mushroom mushroom )
			{
				_mushrooms.Remove( mushroom );
			}
		}

		public async UniTask HealAll()
		{
			await UniTask.WhenAll(
				_mushrooms.Select( mushroom => mushroom.Heal() )
			);
		}
	}
}