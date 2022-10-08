using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Block : MonoBehaviour,
		IDamageable
	{
		private HealthController _health;

		[Inject]
		public void Construct( HealthController health )
		{
			_health = health;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			int dmgDealt = _health.TakeDamage( data );

			Debug.LogFormat( data.LogFormat(), name, dmgDealt, instigator?.name, causer?.name );

			return dmgDealt;
		}
	}
}
