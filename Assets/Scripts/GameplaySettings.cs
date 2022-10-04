using Minipede.Gameplay.Movement;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class GameplaySettings : ScriptableObjectInstaller
    {
		[SerializeField] private Player _playerSettings;

		public override void InstallBindings()
		{
			Container.BindInstance( _playerSettings );
		}

		[System.Serializable]
        public class Player
		{
			public CharacterMotor.Settings Movement;
		}
	}
}
