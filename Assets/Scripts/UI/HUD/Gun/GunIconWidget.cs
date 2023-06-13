using Minipede.Gameplay.Player;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GunIconWidget : MonoBehaviour
	{
		[SerializeField] private MonoSpriteWidget _icon;

		private EquippedGunModel _gunModel;

		[Inject]
		public void Construct( EquippedGunModel gunModel )
		{
			_gunModel = gunModel;
		}

		private void OnDisable()
		{
			_gunModel.Changed -= ChangeIcon;
		}

		private void OnEnable()
		{
			_gunModel.Changed += ChangeIcon;

			ChangeIcon( _gunModel.Gun );
		}

		private void ChangeIcon( Gun newGun )
		{
			_icon.SetSprite( newGun?.Icon );
		}
	}
}