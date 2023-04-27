using Rewired;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace ControllerGlyph.Editor
{
	[System.Serializable]
	public class MouseGlyphImporter
	{
		[BoxGroup( "Raw Data" ), OnValueChanged( "Ingest" )]
		public TextAsset RewiredCsv;
		[BoxGroup( "Raw Data" )]
		public int FirstEntryLine = 3;

		public MouseElements Mouse;

		public void Ingest()
		{
			if ( RewiredCsv == null )
			{
				return;
			}

			Mouse = new MouseElements();
			Mouse.Elements = new List<MouseElementId>();

			string[] lines = RewiredCsv.text.Split( '\n' );
			for ( int idx = FirstEntryLine - 1; idx < lines.Length - 1; ++idx ) // Ignore last line since it's empty.
			{
				int d = 0;
				string[] data = lines[idx].Split( "," );

				MouseElementId elementId = new MouseElementId();
				elementId.Element = new ControllerElementId()
				{
					Id = int.Parse( data[d++] ),
					Name = data[d++]
				};
				elementId.PositiveName = data[d++];
				elementId.NegativeName = data[d++];
				elementId.Element.Type = (ControllerElementType)int.Parse( data[d++] );

				Mouse.Elements.Add( elementId );
			}
		}
	}
}