using System.Collections.Generic;

namespace Minipede.Gameplay.UI
{
	public class MenuStack
	{
		public bool IsEmpty => _menus.Count <= 0;
		public IMenu Top => _menus.Peek();

		private readonly Stack<IMenu> _menus;

		public MenuStack()
		{
			_menus = new Stack<IMenu>();
		}

		public void Clear()
		{
			while ( !IsEmpty )
			{
				Pop();
			}
		}

		public void Pop()
		{
			var topMenu = _menus.Pop();
			topMenu.Hide();

			if ( _menus.TryPeek( out topMenu ) )
			{
				topMenu.Show();
			}
		}

		public void Open( IMenu menu )
		{
			if ( _menus.TryPeek( out var topMenu ) )
			{
				topMenu.Hide();
			}

			_menus.Push( menu );
			menu.Show();
		}
	}
}