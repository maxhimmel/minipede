using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public interface IAttack
	{
		void SetOwner( Transform owner );
	}
}