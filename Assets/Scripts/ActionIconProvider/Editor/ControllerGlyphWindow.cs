using System.Collections.Generic;
using Rewired;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
	public class ControllerGlyphWindow : OdinEditorWindow
	{
		public TextAsset RewiredCsv;
		public int FirstEntryLine = 3;

		[Searchable( FilterOptions = SearchFilterOptions.ISearchFilterableInterface, Recursive = false )]
		[ListDrawerSettings( IsReadOnly = true, ListElementLabelName = "EditorControllerName", ShowIndexLabels = true )]
		public List<ControllerElements> ControllerElements;

		[MenuItem( "Tools/Input/Controller Glyph Importer" )]
		private static void OpenWindow()
		{
			GetWindow<ControllerGlyphWindow>().Show();
		}

		protected override void Initialize()
		{
			base.Initialize();

			ControllerElements = new List<ControllerElements>();

			string parsedCsv = RewiredCsv.text.Replace( "\"", "" );

			string[] lines = parsedCsv.Split( '\n' );
			for ( int idx = FirstEntryLine - 1; idx < lines.Length - 1; ++idx ) // Ignore last line since it's empty.
			{
				int d = 0;
				string[] data = lines[idx].Split( "," );

				ControllerElementId elementId = new ControllerElementId();
				elementId.EditorControllerName = data[d++];
				elementId.ControllerName = data[d++];
				elementId.ControllerGuid = data[d++];
				elementId.ElementId = int.Parse( data[d++] );
				elementId.ElementIdName = data[d++];
				elementId.ElementIdPositiveName = data[d++];
				elementId.ElementIdNegativeName = data[d++];
				elementId.ElementIdType = (ControllerElementType)int.Parse( data[d++] );

				ControllerElements controller = ControllerElements.Find( c => c.ControllerGuid == elementId.ControllerGuid );
				if ( controller == null )
				{
					controller = new ControllerElements();
					controller.EditorControllerName = elementId.EditorControllerName;
					controller.ControllerName = elementId.ControllerName;
					controller.ControllerGuid = elementId.ControllerGuid;
					controller.Elements = new List<ControllerElementId>();

					ControllerElements.Add( controller );
				}

				controller.Elements.Add( elementId );
			}

			ControllerElements.Sort( ( lhs, rhs ) => lhs.ControllerName.CompareTo( rhs.ControllerName ) );
		}
	}
}