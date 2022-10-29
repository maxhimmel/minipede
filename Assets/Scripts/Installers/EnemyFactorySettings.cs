using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.LevelPieces;
using UnityEngine.Serialization;

namespace Minipede.Installers
{
    public partial class EnemySettings
	{
		// TODO: All of these could be private classes ...

		[System.Serializable]
		public struct Bee
		{
			public BeeController Prefab;
			public GraphSpawnPlacement[] SpawnPlacement;
			public BeeController.Settings Settings;
		}

		[System.Serializable]
		public struct Mosquito
		{
			public MosquitoController Prefab;
			public GraphSpawnPlacement[] SpawnPlacement;
			public MosquitoController.Settings Settings;
		}

		[System.Serializable]
		public struct Earwig
		{
			public EarwigController Prefab;
			public GraphSpawnPlacement[] SpawnPlacement;
		}

		[System.Serializable]
		public struct Dragonfly
		{
			public DragonflyController Prefab;
			public GraphSpawnPlacement[] SpawnPlacement;
			public DragonflyController.Settings Settings;
		}

		[System.Serializable]
		public struct Beetle
		{
			public BeetleController Prefab;
			public GraphSpawnPlacement[] SpawnPlacement;
			public BeetleController.Settings Settings;
		}

		[System.Serializable]
		public struct Minipede
		{
			public MinipedeController Prefab;
			public GraphSpawnPlacement[] SpawnPlacement;
			public MinipedeController.Settings Settings;
		}
	}
}
