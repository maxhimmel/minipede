using System.Collections.Generic;
using Minipede.Utility;

namespace Minipede.Gameplay.Weapons
{
    public interface IFireSpread : IGunModule
    {
        IEnumerable<IOrientation> GetSpread( ShotSpot shotSpot );
    }
}
