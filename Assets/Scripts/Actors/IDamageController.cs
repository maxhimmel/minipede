using System;

namespace Minipede.Gameplay
{
    public interface IDamageController : IDamageable
    {
        event EventHandler<HealthController> Damaged;
        event EventHandler<HealthController> Died;
    }
}
