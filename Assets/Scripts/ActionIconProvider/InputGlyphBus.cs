using System.Collections.Generic;
using System.Linq;
using Rewired;

namespace Minipede
{
	public class InputGlyphBus
	{
		private readonly Settings _settings;
		private readonly Dictionary<string, InputGlyphs> _glyphs;

		public InputGlyphBus( Settings settings )
		{
			_settings = settings;
			_glyphs = settings.Glyphs.ToDictionary( g => g.InputGuid );
		}

		public string GetGlyph( int actionId, AxisRange axisRange = AxisRange.Full )
		{
			string controllerGuid = "";
			int elementId = actionId;

			if ( !_glyphs.TryGetValue( controllerGuid, out var glyph ) )
			{
				glyph = _settings.Fallback;
			}

			return glyph.GetGlyph( elementId, axisRange );
		}

		[System.Serializable]
		public class Settings
		{
			public InputGlyphs Fallback;
			public InputGlyphs[] Glyphs;
		}
	}
}