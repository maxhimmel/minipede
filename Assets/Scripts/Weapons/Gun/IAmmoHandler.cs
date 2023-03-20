namespace Minipede.Gameplay.Weapons
{
	public interface IAmmoHandler : IFireSafety,
		IFireEndProcessor
	{
		event System.Action Emptied;

		void Reload();
	}
}