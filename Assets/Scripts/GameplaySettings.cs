using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

using BlockPiece = Minipede.Gameplay.LevelPieces.Block;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class GameplaySettings : ScriptableObjectInstaller
    {
		[SerializeField] private Player _playerSettings;
		[SerializeField] private Block _blockSettings;
		[SerializeField] private Level _levelSettings;

		public override void InstallBindings()
		{
			Container.BindInstance( _playerSettings );

			Container.Bind<HealthController>()
				.WithArguments( _blockSettings.Health )
				.WhenInjectedInto<BlockPiece>();

			Container.BindInstance( _levelSettings );

			Container.BindFactory<BlockPiece, BlockPiece.Factory>()
				.FromComponentInNewPrefab( _levelSettings.BlockPrefab );
		}

		[System.Serializable]
        public struct Player
		{
			public CharacterMotor.Settings Movement;
			public HealthController.Settings Health;
		}

		[System.Serializable]
		public struct Block
		{
			public HealthController.Settings Health;
		}

		[System.Serializable]
		public struct Level
		{
			public BlockPiece BlockPrefab;
			public LevelGraph.Settings Graph;

			[TabGroup("Blocks Per Row")]
			public WeightedListInt RowGeneration;
		}
	}
}
