using System.Collections.Generic;
using System.Linq;
using Minipede.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class ActionGlyphController : ILateTickable
	{
		private readonly Settings _settings;
		private readonly Transform _transform;
		private readonly Rigidbody2D _root;
		private readonly ExplorerController _explorerController;
		private readonly Dictionary<int, ActionGlyphPrompt> _prompts;

		public ActionGlyphController( Settings settings,
			Transform transform,
			Rigidbody2D root,
			ActionGlyphPrompt[] prompts,
			ExplorerController explorerController )
		{
			_settings = settings;
			_transform = transform;
			_root = root;
			_explorerController = explorerController;
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
			OrientateView();
			MoveExplorerOffset();
		}

		private void OrientateView()
		{
			_transform.rotation = Quaternion.identity;
		}

		private void MoveExplorerOffset()
		{
			if ( _explorerController.Pawn != null )
			{
				Vector2 dirAwayFromExplorer = _root.position - _explorerController.Pawn.Orientation.Position;
				float distFromExplorer = dirAwayFromExplorer.magnitude;

				float offset = _settings.DistanceCurve.Evaluate( distFromExplorer );
				Vector2 offsetDir = Mathf.Approximately( distFromExplorer, 0 )
					? Vector2.down
					: dirAwayFromExplorer / distFromExplorer;

				_transform.position = _root.position + offsetDir * offset;
			}
		}

		[System.Serializable]
		public class Settings
		{
			[InfoBox( "<b>X Axis:</b> distance from explorer \n<b>Y Axis:</b> glyph offset radius" )]
			public AnimationCurve DistanceCurve = AnimationCurve.EaseInOut( 0.5f, 0.75f, 1.5f, 0.2f );
		}
	}
}