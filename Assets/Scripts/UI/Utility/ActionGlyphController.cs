using System.Collections.Generic;
using System.Linq;
using Rewired;

namespace Minipede.Gameplay.UI
{
	public class ActionGlyphController
	{
		private readonly Dictionary<int, ActionGlyphPrompt> _prompts;

		public ActionGlyphController( ActionGlyphPrompt[] prompts )
		{
			_prompts = prompts.ToDictionary( p => p.ActionId );

			HideAll();
		}

		public void HideAll()
		{
			foreach ( var prompt in _prompts.Values )
			{
				prompt.Hide();
			}
		}

		public void HideAction( int id )
		{
			_prompts[id].Hide();
		}

		public void ShowAll()
		{
			foreach ( var prompt in _prompts.Values )
			{
				prompt.Show();
			}
		}

		public void ShowAction( int id )
		{
			_prompts[id].Show();
		}
	}
}