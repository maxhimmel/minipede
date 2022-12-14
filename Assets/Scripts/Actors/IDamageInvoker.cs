using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
    public interface IDamageInvoker
    {
        DamageResult Invoke( IDamageable damageable, Transform instigator, Transform causer );

        public interface ISettings
        {
            /// <summary>
            /// Must be of type <see cref="IDamageInvoker"/>.
            /// </summary>
            System.Type DamageType { get; }
        }

		public class Factory
        {
            private readonly DiContainer _container;

            public Factory( DiContainer container )
            {
                _container = container;
            }

            public IDamageInvoker Create( ISettings settings )
            {
				return _container.Instantiate( settings.DamageType, new object[] { settings } )
					as IDamageInvoker;
			}
        }
    }
}