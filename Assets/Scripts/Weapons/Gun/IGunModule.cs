using System;
using Sirenix.Utilities;

namespace Minipede.Gameplay.Weapons
{
    public interface IGunModule
    {
        Type ModuleType { get; }

        string GetModuleLabel()
        {
            return ModuleType.Name.SplitPascalCase();
        }
    }
}
