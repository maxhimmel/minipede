using Minipede.Gameplay.Weapons;

namespace Minipede.Gameplay.Player
{
	public class EquippedGunModel
	{
		public event System.Action<Gun> Changed;

		public Gun Gun { get; private set; }

		public void SetGun( Gun newGun )
		{
			if ( Gun != newGun )
			{
				Gun = newGun;
				Changed?.Invoke( newGun );
			}
		}
	}
}