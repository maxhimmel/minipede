using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class SettingsMenu : Menu
	{
		[Header( "Buttons" )]
		[SerializeField] private Button _audioButton;
		[SerializeField] private Button _backButton;

		public override void Initialize()
		{
			base.Initialize();

			_audioButton.onClick.AddListener( _menuController.Open<AudioMenu> );
			_backButton.onClick.AddListener( _menuController.Pop );
		}
	}
}