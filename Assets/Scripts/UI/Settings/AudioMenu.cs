using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class AudioMenu : Menu
	{
		[SerializeField] private Button _backButton;

		[Header( "SFX" )]
		[SerializeField] private Button _reduceSfxButton;
		[SerializeField] private Button _increaseSfxButton;

		[Header( "Music" )]
		[SerializeField] private Button _reduceMusicButton;
		[SerializeField] private Button _increaseMusicButton;

		public override void Initialize()
		{
			base.Initialize();

			_backButton.onClick.AddListener( _menuController.Pop );
		}
	}
}