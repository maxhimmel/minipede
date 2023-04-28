using System.Collections.Generic;
using System.Linq;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ControllerGlyph
{
	public abstract class ControllerGlyphs : ScriptableObject
	{
		public abstract string InputGuid { get; }
		public abstract ControllerType ControllerType { get; }

		[BoxGroup( "Sprite", ShowLabel = false )]
		[SerializeField] private string _spriteAssetName;
		[BoxGroup( "Sprite", ShowLabel = false )]
		[SerializeField] private Style _spriteStyle;

		public string GetGlyph( Request request )
		{
			int elementId = GetElementId( request );
			return GetGlyph( elementId, request.AxisRange );
		}

		protected virtual int GetElementId( Request request )
		{
			var elementMaps = request.Input
				.controllers
				.maps
				.ElementMapsWithAction( ControllerType, request.ActionId, skipDisabledMaps: true );

			var action = SelectElement( elementMaps, request.ElementType, request.AxisRange );

			return action != null
				? action.elementIdentifierId
				: -1;
		}

		protected virtual ActionElementMap SelectElement( IEnumerable<ActionElementMap> elementMaps, ControllerElementType type, AxisRange axisRange = AxisRange.Full )
		{
			return elementMaps.FirstOrDefault( aem =>
			{
				if ( axisRange == AxisRange.Full )
				{
					return true;
				}
				else if ( aem.axisContribution == Pole.Positive && axisRange == AxisRange.Positive )
				{
					return true;
				}
				else if ( aem.axisContribution == Pole.Negative && axisRange == AxisRange.Negative )
				{
					return true;
				}

				return false;
			} );
		}

		public abstract string GetGlyph( int elementId, AxisRange axisRange = AxisRange.Full );

		protected string GetRtf( int spriteIndex )
		{
			string style = _spriteStyle == Style.Filled
				? "-Filled"
				: string.Empty;

			return $"<sprite=\"{_spriteAssetName}{style}\" index={spriteIndex}>";
		}

		private enum Style
		{
			Filled,
			Transparent
		}

		public class Request
		{
			public Player Input { get; }
			public int ActionId { get; }
			public ControllerElementType ElementType { get; }
			public AxisRange AxisRange { get; }

			public Request( Player input,
				int actionId,
				ControllerElementType elementType,
				AxisRange axisRange )
			{
				Input = input;
				ActionId = actionId;
				ElementType = elementType;
				AxisRange = axisRange;
			}
		}
	}
}