using Minipede.Gameplay.Player;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GunColorWidget : MonoBehaviour
	{
		[SerializeField] private MonoColorWidget _color;
		[SerializeField] private Color _fallbackColor = Color.clear;

		private EquippedGunModel _gunModel;

		[Inject]
		public void Construct( EquippedGunModel gunModel )
		{
			_gunModel = gunModel;
		}

		private void OnEnable()
		{
			_gunModel.Changed += UpdateColor;

			UpdateColor( _gunModel.Gun );
		}

		private void OnDisable()
		{
			_gunModel.Changed -= UpdateColor;
		}

		private void UpdateColor( Gun newGun )
		{
			Color color = _fallbackColor;

			if ( newGun != null && newGun.Type != null )
			{
				color = newGun.Type.Color;
			}

			_color.SetColor( color );
		}
	}
}