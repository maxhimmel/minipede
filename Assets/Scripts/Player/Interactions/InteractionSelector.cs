using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
	public class InteractionSelector : MonoBehaviour
	{
		public ISelectable Selectable { get; private set; }

		private List<ISelectable> _selectablesWithinRange = new List<ISelectable>();

		private void OnTriggerEnter2D( Collider2D collision )
		{
			if ( !collision.TryGetComponentFromBody<ISelectable>( out var otherSelectable ) )
			{
				return;
			}
			if ( !otherSelectable.CanBeInteracted() )
			{
				return;
			}

			if ( Selectable == null )
			{
				Select( otherSelectable );
			}

			_selectablesWithinRange.Add( otherSelectable );
		}

		private void OnTriggerExit2D( Collider2D collision )
		{
			if ( collision.TryGetComponentFromBody<ISelectable>( out var otherSelectable ) )
			{
				if ( Selectable == otherSelectable )
				{
					Selectable.Deselect();
					Selectable = null;
				}

				_selectablesWithinRange.Remove( otherSelectable );
			}
		}

		private void Update()
		{
			if ( _selectablesWithinRange.Count <= 1 )
			{
				return;
			}

			if ( TryGetClosestSelectable( out var closest ) )
			{
				Select( closest );
			}
		}

		private bool TryGetClosestSelectable( out ISelectable closest )
		{
			float distSqrToCurrent = (Selectable.Orientation.Position - transform.position.ToVector2()).sqrMagnitude;
			float closestDistSqr = distSqrToCurrent;
			closest = null;

			foreach ( var otherSelectable in _selectablesWithinRange )
			{
				if ( otherSelectable == Selectable )
				{
					// Skip what's currently selected ...
					continue;
				}

				float distSqrToOther = (otherSelectable.Orientation.Position - transform.position.ToVector2()).sqrMagnitude;

				if ( distSqrToOther < closestDistSqr )
				{
					closestDistSqr = distSqrToOther;
					closest = otherSelectable;
				}
			}

			return closest != null;
		}

		private void Select( ISelectable selectable )
		{
			if ( Selectable != null )
			{
				Selectable.Deselect();
			}

			Selectable = selectable;
			Selectable.Select();
		}
	}
}