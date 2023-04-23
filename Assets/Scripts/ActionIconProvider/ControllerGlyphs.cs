using System.Collections.Generic;
using Rewired;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede
{
	[CreateAssetMenu( menuName = "Tools/Input/Controller Glyph Asset" )]
	public class ControllerGlyphs : ScriptableObject
	{
		public string ControllerGuid => _controllerGuid;

		[SerializeField] private string _editorControllerName;
		[SerializeField] private string _controllerName;
		[SerializeField] private string _controllerGuid;

		[ListDrawerSettings( IsReadOnly = true, ShowPaging = false )]
		[SerializeField] private List<ElementGlyph> _glyphs = new List<ElementGlyph>();

		public void Construct( string editorControllerName,
			string controllerName,
			string controllerGuid,
			IList<(int id, string name)> elementIds )
		{
			_editorControllerName = editorControllerName;
			_controllerName = controllerName;
			_controllerGuid = controllerGuid;

			_glyphs = new List<ElementGlyph>( elementIds.Count );
			foreach ( var e in elementIds )
			{
				_glyphs.Add( new ElementGlyph()
				{
					ElementId = e.id,
					Name = e.name
				} );
			}
		}

		public void Example()
		{
			var controller = ReInput.players.GetPlayer( 0 )
				.controllers.GetLastActiveController();

			var actionId = 0;

			var actionElementMap = ReInput.players.GetPlayer( 0 )
				.controllers
				.maps
				.GetFirstElementMapWithAction( controller, actionId, skipDisabledMaps: true );

			// this is what we want ...
			//controller.hardwareTypeGuid
			//actionElementMap.elementIdentifierId
			//actionElementMap.axisRange
		}

		public Sprite GetGlyph( int elementId, AxisRange axisRange = AxisRange.Full )
		{
			var element = _glyphs.Find( e => e.ElementId == elementId );
			switch ( axisRange )
			{
				default:
				case AxisRange.Full: 
					return element.Glyph;

				case AxisRange.Positive: 
					return element.GlyphPositive;
				case AxisRange.Negative: 
					return element.GlyphNegative;
			}
		}

		[System.Serializable]
		private class ElementGlyph
		{
			private const float _linkWidth = 40;

			[BoxGroup( "All", ShowLabel = false )]

			[HorizontalGroup( "All/Title", Gap = 0 )]
			[BoxGroup( "All/Title/Name" )]
			[ReadOnly, HideLabel]
			public string Name;
			[BoxGroup( "All/Title/ID" )]
			[ReadOnly, HideLabel]
			public int ElementId;

			[BoxGroup( "All" ), OnValueChanged( "OnMainGlyphChanged" )]
			[PreviewField( Alignment = Sirenix.OdinInspector.ObjectFieldAlignment.Right )]
			public Sprite Glyph;
			
			[HorizontalGroup( "All/Positive" )]
			public Sprite GlyphPositive;
			[HideInInspector]
			public bool IsPositiveLinked = true;

			[HorizontalGroup( "All/Negative" )]
			public Sprite GlyphNegative;
			[HideInInspector]
			public bool IsNegativeLinked = true;

			private void OnMainGlyphChanged()
			{
				if ( IsPositiveLinked )
				{
					GlyphPositive = Glyph;
				}
				if ( IsNegativeLinked )
				{
					GlyphNegative = Glyph;
				}
			}

			[HorizontalGroup( "All/Positive", Width = _linkWidth ), OnInspectorGUI]
			private void TogglePositiveLink()
			{
				Undo.RecordObject( Selection.activeObject, "Toggle Positive Glyph Link" );
				IsPositiveLinked = ToggleGlyphLink( IsPositiveLinked );
			}

			[HorizontalGroup( "All/Negative", Width = _linkWidth ), OnInspectorGUI]
			private void ToggleNegativeLink()
			{
				Undo.RecordObject( Selection.activeObject, "Toggle Negative Glyph Link" );
				IsNegativeLinked = ToggleGlyphLink( IsNegativeLinked );
			}

			private bool ToggleGlyphLink( bool linkedState )
			{
				return SirenixEditorGUI.ToolbarToggle( linkedState, EditorIcons.Link );
			}
		}
	}
}
