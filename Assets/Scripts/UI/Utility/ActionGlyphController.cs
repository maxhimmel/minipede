using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class ActionGlyphController : ILateTickable
	{
		private readonly Transform _transform;
		private readonly Dictionary<int, ActionGlyphPrompt> _prompts;

		public ActionGlyphController( Transform transform,
			ActionGlyphPrompt[] prompts )
		{
			_transform = transform;
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

		public void LateTick()
		{
			_transform.rotation = Quaternion.identity;
		}
	}
}