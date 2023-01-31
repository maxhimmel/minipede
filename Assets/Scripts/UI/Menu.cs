using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class Menu : MonoBehaviour,
		IMenu
	{
		public string Title => _title;

		[SerializeField] private string _title = "Menu";

		protected SignalBus _signalBus;
		protected MenuController _menuController;

		[Inject]
		public void Construct( SignalBus signalBus,
			MenuController menuController )
		{
			_signalBus = signalBus;
			_menuController = menuController;
		}

		public virtual void Initialize()
		{

		}

		public virtual void Show()
		{
			gameObject.SetActive( true );
		}

		public virtual void Hide()
		{
			gameObject.SetActive( false );
		}
	}
}