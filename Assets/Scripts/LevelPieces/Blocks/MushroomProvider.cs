namespace Minipede.Gameplay.LevelPieces
{
	public class MushroomProvider
	{
		private readonly BlockFactoryBus.Settings _settings;

		public MushroomProvider( BlockFactoryBus.Settings settings )
		{
			_settings = settings;
		}

		public Mushroom GetStandardAsset()
		{
			return _settings.Standard.Prefab;
		}

		public Mushroom GetPoisonAsset()
		{
			return _settings.Poison.Prefab;
		}

		public Mushroom GetFlowerAsset()
		{
			return _settings.Flower.Prefab;
		}
	}
}