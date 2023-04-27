using Rewired;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ControllerGlyph.Editor
{
	[System.Serializable]
	public class KeyboardGlyphImporter
	{
		[BoxGroup( "Raw Data" ), OnValueChanged( "Ingest" )]
		public Object RewiredTsv;
		[BoxGroup( "Raw Data" )]
		public int FirstEntryLine = 3;

		public KeyboardElements Keyboard;

		public void Ingest()
		{
			if ( RewiredTsv == null )
			{
				return;
			}

			Keyboard = new KeyboardElements();
			Keyboard.Elements = new List<KeyboardElementId>();

			var projectPath = Application.dataPath.Remove( Application.dataPath.Length - "Assets".Length );
			var assetPath = AssetDatabase.GetAssetPath( RewiredTsv );

			string[] lines = System.IO.File.ReadAllLines( projectPath + assetPath );
			for ( int idx = FirstEntryLine - 1; idx < lines.Length; ++idx )
			{
				int d = 0;
				string[] data = lines[idx].Split( "\t" );

				KeyboardElementId elementId = new KeyboardElementId();
				elementId.Element = new ControllerElementId()
				{
					Id = int.Parse( data[d++] )
				};
				elementId.KeyCode = int.Parse( data[d++] );
				elementId.Element.Name = data[d++];
				elementId.Element.Type = (ControllerElementType)int.Parse( data[d++] );

				Keyboard.Elements.Add( elementId );
			}
		}
	}
}