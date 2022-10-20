using Minipede.Gameplay.Enemies;

namespace Minipede.Installers
{
    public partial class EnemySettings
	{
		[System.Serializable]
		public struct Bee
		{
			public BeeController Prefab;
			public BeeController.Settings Settings;
		}

		[System.Serializable]
		public struct Mosquito
		{
			public MosquitoController Prefab;
			public MosquitoController.Settings Settings;
		}

		[System.Serializable]
		public struct Earwig
		{
			public EarwigController Prefab;
		}

		[System.Serializable]
		public struct Dragonfly
		{
			public DragonflyController Prefab;
			public DragonflyController.Settings Settings;
		}

		[System.Serializable]
		public struct Beetle
		{
			public BeetleController Prefab;
			public BeetleController.Settings Settings;
		}

		[System.Serializable]
		public struct Minipede
		{
			public MinipedeController Prefab;
			public MinipedeController.Settings Settings;
		}
	}
}
