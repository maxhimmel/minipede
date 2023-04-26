using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede
{
	public abstract class ControllerGlyphs : ScriptableObject
	{
		public abstract string InputGuid { get; }

		[BoxGroup( "Sprite", ShowLabel = false )]
		[SerializeField] private string _spriteAssetName;
		[BoxGroup( "Sprite", ShowLabel = false )]
		[SerializeField] private Style _spriteStyle;

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
	}
}