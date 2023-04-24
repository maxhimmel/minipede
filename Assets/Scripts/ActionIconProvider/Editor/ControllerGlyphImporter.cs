﻿using System.Collections.Generic;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Editor
{
	[System.Serializable]
	public class ControllerGlyphImporter
	{
		[BoxGroup( "Raw Data" ), OnValueChanged( "Ingest" )]
		public TextAsset RewiredCsv;
		[BoxGroup( "Raw Data" )]
		public int FirstEntryLine = 3;

		[Searchable( FilterOptions = SearchFilterOptions.ISearchFilterableInterface, Recursive = false )]
		[ListDrawerSettings( IsReadOnly = true, ListElementLabelName = "@Identifier.EditorControllerName", ShowIndexLabels = true )]
		public List<ControllerElements> ControllerElements;

		public void Ingest()
		{
			if ( RewiredCsv == null )
			{
				return;
			}

			ControllerElements = new List<ControllerElements>();

			string parsedCsv = RewiredCsv.text.Replace( "\"", "" );

			string[] lines = parsedCsv.Split( '\n' );
			for ( int idx = FirstEntryLine - 1; idx < lines.Length - 1; ++idx ) // Ignore last line since it's empty.
			{
				int d = 0;
				string[] data = lines[idx].Split( "," );

				ControllerElementId elementId = new ControllerElementId();
				elementId.ControllerIdentifier = new ControllerIdentifier()
				{
					EditorControllerName = data[d++],
					ControllerName = data[d++],
					ControllerGuid = data[d++]
				};
				elementId.Element = new InputElementId()
				{
					Id = int.Parse( data[d++] ),
					Name = data[d++]
				};
				elementId.PositiveName = data[d++];
				elementId.NegativeName = data[d++];
				elementId.Element.Type = (ControllerElementType)int.Parse( data[d++] );

				ControllerElements controller = ControllerElements.Find(
					c => c.Identifier.ControllerGuid == elementId.ControllerIdentifier.ControllerGuid
				);

				if ( controller == null )
				{
					controller = new ControllerElements();
					controller.Identifier = elementId.ControllerIdentifier;
					controller.Elements = new List<ControllerElementId>();

					ControllerElements.Add( controller );
				}

				controller.Elements.Add( elementId );
			}

			ControllerElements.Sort( ( lhs, rhs ) => lhs.Identifier.ControllerName.CompareTo( rhs.Identifier.ControllerName ) );
		}
	}
}