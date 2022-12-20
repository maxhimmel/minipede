using Minipede.Utility;

namespace Minipede.Gameplay.Treasures
{
    public class Treasure : Collectable<Treasure>
    {
		protected override Treasure GetCollectable()
		{
			return this;
		}
	}
}
