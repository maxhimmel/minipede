using System.Collections.Generic;
using Minipede.Utility;

namespace Minipede.Gameplay.Weapons
{
    public interface IFireSpread
    {
        IEnumerable<IOrientation> GetSpread();
    }
}
