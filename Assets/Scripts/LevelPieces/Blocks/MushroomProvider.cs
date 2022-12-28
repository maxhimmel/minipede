namespace Minipede.Gameplay.LevelPieces
{
	public class MushroomProvider
	{
		private readonly Settings _settings;

		public MushroomProvider( Settings settings )
		{
			_settings = settings;
		}

		public Mushroom GetStandardAsset()
		{
			return _settings.Standard;
		}

		public Mushroom GetPoisonAsset()
		{
			return _settings.Poison;
		}

		public Mushroom GetFlowerAsset()
		{
			return _settings.Flower;
		}

		[System.Serializable]
		public struct Settings
		{
			public Mushroom Standard;
			public PoisonMushroom Poison;
			public Mushroom Flower;
		}
	}
}