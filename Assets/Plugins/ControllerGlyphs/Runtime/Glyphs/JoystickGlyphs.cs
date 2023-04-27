using System.Collections.Generic;
using System.Linq;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
#endif

namespace ControllerGlyph
{
	[CreateAssetMenu( menuName = "Tools/Input/Joystick Glyph Asset" )]
	public class JoystickGlyphs : ControllerGlyphs
	{
		public override string InputGuid => _identifier.ControllerGuid;
		public override ControllerType ControllerType => ControllerType.Joystick;

		[HideLabel]
		[SerializeField] private Core.ControllerIdentifier _identifier;

		[Space, ListDrawerSettings( IsReadOnly = true, ShowPaging = false )]
		[SerializeField] private List<ElementGlyph> _glyphs = new List<ElementGlyph>();

		public void Construct( Core.ControllerIdentifier identifier,
			IList<(int id, string name)> elementIds )
		{
			_identifier = identifier;

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

		protected override ActionElementMap SelectElement( IEnumerable<ActionElementMap> elementMaps, ControllerElementType type, AxisRange axisRange = AxisRange.Full )
		{
			if ( type == ControllerElementType.Axis || type == ControllerElementType.CompoundElement )
			{
				var action = elementMaps.FirstOrDefault( aem => aem.elementType == ControllerElementType.Axis );
				if ( action != null )
				{
					return action;
				}
			}

			return base.SelectElement( elementMaps, type, axisRange );
		}

		public override string GetGlyph( int elementId, AxisRange axisRange = AxisRange.Full )
		{
			var element = _glyphs.Find( e => e.ElementId == elementId );
			if ( element == null || element.ElementId < 0 )
			{
				return GetRtf( -1 );
			}

			switch ( axisRange )
			{
				default:
				case AxisRange.Full:
					return GetRtf( element.SpriteIndex );

				case AxisRange.Positive:
					return GetRtf( element.SpriteIndexPositive );
				case AxisRange.Negative:
					return GetRtf( element.SpriteIndexNegative );
			}
		}

		#region Editor (integration test)
#if UNITY_EDITOR
		[Button( ButtonSizes.Gigantic, Icon = SdfIconType.BookmarkCheck, IconAlignment = IconAlignment.LeftEdge )]
		private void Test()
		{
			GameObject testObj = new GameObject( name );
			var tmp = testObj.AddComponent<TMPro.TextMeshPro>();
			tmp.enableWordWrapping = false;

			foreach ( var element in _glyphs )
			{
				tmp.text += $"{GetRtf( element.SpriteIndex )} : {element.Name}\n";
				if ( element.SpriteIndexPositive != element.SpriteIndex )
				{
					tmp.text += $"   {GetRtf( element.SpriteIndexPositive )} : Positive\n";
				}
				if ( element.SpriteIndexNegative != element.SpriteIndex )
				{
					tmp.text += $"   {GetRtf( element.SpriteIndexNegative )} : Negative\n";
				}
			}
		}
#endif
		#endregion

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

			[BoxGroup( "All/Sprite" ), OnValueChanged( "OnMainGlyphChanged" )]
			[LabelText( "Index" )]
			public int SpriteIndex;
			
			[HorizontalGroup( "All/Sprite/Positive" )]
			[LabelText( "Positive" )]
			public int SpriteIndexPositive;
			[HideInInspector]
			public bool IsPositiveLinked = true;

			[HorizontalGroup( "All/Sprite/Negative" )]
			[LabelText( "Negative" )]
			public int SpriteIndexNegative;
			[HideInInspector]
			public bool IsNegativeLinked = true;

			#region Editor
#if UNITY_EDITOR
			private void OnMainGlyphChanged()
			{
				if ( IsPositiveLinked )
				{
					SpriteIndexPositive = SpriteIndex;
				}
				if ( IsNegativeLinked )
				{
					SpriteIndexNegative = SpriteIndex;
				}
			}

			[HorizontalGroup( "All/Sprite/Positive", Width = _linkWidth ), OnInspectorGUI]
			private void TogglePositiveLink()
			{
				Undo.RecordObject( Selection.activeObject, "Toggle Positive Glyph Link" );
				IsPositiveLinked = ToggleGlyphLink( IsPositiveLinked );
			}

			[HorizontalGroup( "All/Sprite/Negative", Width = _linkWidth ), OnInspectorGUI]
			private void ToggleNegativeLink()
			{
				Undo.RecordObject( Selection.activeObject, "Toggle Negative Glyph Link" );
				IsNegativeLinked = ToggleGlyphLink( IsNegativeLinked );
			}

			private bool ToggleGlyphLink( bool linkedState )
			{
				return SirenixEditorGUI.ToolbarToggle( linkedState, EditorIcons.Link );
			}
#endif
			#endregion
		}
	}
}