using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.LevelPieces;

namespace Minipede.Installers
{
    public partial class EnemySettings
	{
		[System.Serializable]
		public struct Bee
		{
			public BeeController Prefab;
			public GraphArea[] SpawnPlacement;
			public BeeController.Settings Settings;
		}

		[System.Serializable]
		public struct Mosquito
		{
			public MosquitoController Prefab;
			public GraphArea[] SpawnPlacement;
			public MosquitoController.Settings Settings;
		}

		[System.Serializable]
		public struct Earwig
		{
			public EarwigController Prefab;
			public GraphArea[] SpawnPlacement;
		}

		[System.Serializable]
		public struct Dragonfly
		{
			public DragonflyController Prefab;
			public GraphArea[] SpawnPlacement;
			public DragonflyController.Settings Settings;
		}

		[System.Serializable]
		public struct Beetle
		{
			public BeetleController Prefab;
			public GraphArea[] SpawnPlacement;
			public BeetleController.Settings Settings;
		}

		[System.Serializable]
		public struct Minipede
		{
			public MinipedeController Prefab;
			public GraphArea[] SpawnPlacement;
			public MinipedeController.Settings Settings;
		}
	}
}
