using System;
using UnityEngine;

namespace Minipede.Gameplay
{
    public interface IDamageController : IDamageable
    {
        event OnHit Damaged;
        event OnHit Died;

        public delegate void OnHit( Rigidbody2D victimBody, HealthController health );
    }
}
