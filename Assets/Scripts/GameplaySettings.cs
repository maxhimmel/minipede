using Minipede.Gameplay;
using Minipede.Gameplay.Movement;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class GameplaySettings : ScriptableObjectInstaller
    {
		[SerializeField] private Player _playerSettings;
		[SerializeField] private Block _blockSettings;

		public override void InstallBindings()
		{
			Container.BindInstance( _playerSettings );

			Container.Bind<HealthController>()
				.WithArguments( _blockSettings.Health )
				.WhenInjectedInto<Gameplay.LevelPieces.Block>();
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
	}
}
