﻿using System.Collections.Generic;
using Rewired;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ControllerGlyph
{
	[CreateAssetMenu( menuName = "Tools/Input/Keyboard Glyph Asset" )]
	public class KeyboardGlyphs : ControllerGlyphs
	{
		public override string InputGuid => ControllerType.ToString();
		public override ControllerType ControllerType => ControllerType.Keyboard;

		[Space, ListDrawerSettings( IsReadOnly = true, ShowPaging = false )]
		[SerializeField] private List<ElementGlyph> _glyphs = new List<ElementGlyph>();

		public void Construct( IList<(int id, string name)> elementIds )
		{
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

		public override string GetGlyph( int elementId, AxisRange axisRange = AxisRange.Full )
		{
			var element = _glyphs.Find( e => e.ElementId == elementId );
			if ( element == null || element.ElementId < 0 )
			{
				return GetRtf( -1 );
			}

			return GetRtf( element.SpriteIndex );
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
			}
		}
#endif
		#endregion

		[System.Serializable]
		private class ElementGlyph
		{
			[BoxGroup( "All", ShowLabel = false )]

			[HorizontalGroup( "All/Title", Gap = 0 )]
			[BoxGroup( "All/Title/Name" )]
			[ReadOnly, HideLabel]
			public string Name;
			[BoxGroup( "All/Title/ID" )]
			[ReadOnly, HideLabel]
			public int ElementId;

			[BoxGroup( "All/Sprite" )]
			[LabelText( "Index" )]
			public int SpriteIndex;
		}
	}
}