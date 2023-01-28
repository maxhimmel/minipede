using Minipede.Gameplay.Enemies;

namespace Minipede.Gameplay.Player
{
    public interface IDangerWarningReaction
    {
        void React( EnemyController enemy );
        void Neglect( EnemyController enemy );

        public interface ISettings
		{
            System.Type InstallerType { get; }
		}
	}
}
