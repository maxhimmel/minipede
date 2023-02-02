using System.Collections.Generic;
using Minipede.Utility;
using TMPro;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] private Canvas _canvas;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Transform _container;

        private MenuStack _menuStack;
        private PlayerInputResolver _inputResolver;
        private Dictionary<System.Type, IMenu> _subMenus;

        [Inject]
		public void Construct( MenuStack menuStack,
            PlayerInputResolver inputResolver )
		{
            _menuStack = menuStack;
            _inputResolver = inputResolver;

            var subMenus = _container.GetComponentsInChildren<IMenu>( includeInactive: true );
            _subMenus = new Dictionary<System.Type, IMenu>( subMenus.Length );

            foreach ( var menu in subMenus )
			{
                _subMenus.Add( menu.GetType(), menu );
                menu.Initialize();
			}

            Clear();
		}

		public void Open<TMenu>()
			where TMenu : IMenu
		{
            _canvas.enabled = true;

            var subMenu = _subMenus[typeof( TMenu )];

            _title.text = subMenu.Title;
            _menuStack.Open( subMenu );
		}

        public void Pop()
		{
            _menuStack.Pop();

            if ( _menuStack.IsEmpty )
			{
                _canvas.enabled = false;
			}
            else
			{
                var topMenu = _menuStack.Top;
                _title.text = topMenu.Title;
			}
		}

        public void Clear()
		{
            _menuStack.Clear();

            if ( _menuStack.IsEmpty )
			{
                _canvas.enabled = false;
			}
        }

		#region "Cancel" button returns to previous menu. *BUGGED* This doesn't unpause the game even though it'll close the menus.
		//      private void Start()
		//      {
		//          var input = _inputResolver.GetInput();
		//          input.AddButtonPressedDelegate( OnBackPressed, ReConsts.Action.Cancel );
		//      }

		//      private void OnBackPressed( Rewired.InputActionEventData obj )
		//      {
		//          Pop();
		//      }

		//      private void OnDestroy()
		//{
		//          var input = _inputResolver.GetInput();
		//          input.RemoveInputEventDelegate( OnBackPressed );
		//}
		#endregion
	}
}
