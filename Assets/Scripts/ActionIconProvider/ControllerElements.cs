using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede
{
	[HideReferenceObjectPicker]
	[System.Serializable]
	public class ControllerElements : ISearchFilterable
	{
		private const string _glyphAssetPath = "Assets/Scripts/ActionIconProvider/Assets";

		public string EditorControllerName;
		public string ControllerName;
		public string ControllerGuid;

		[ListDrawerSettings( IsReadOnly = true, ShowPaging = false )]
		public List<ControllerElementId> Elements;

		bool ISearchFilterable.IsMatch( string searchString )
		{
			searchString = searchString.ToLower();
			return ControllerName.ToLower().Contains( searchString ) || EditorControllerName.ToLower().Contains( searchString );
		}

		[OnInspectorGUI, PropertyOrder( -1 )]
		private void CreateGlyphAsset()
		{
			if ( SirenixEditorGUI.MenuButton( EditorGUI.indentLevel, " Create Glyph Asset", false, EditorIcons.ImageCollection.Raw ) )
			{
				var savePath = EditorUtility.SaveFilePanelInProject(
					"Save Controller Glyph Asset",
					$"{ControllerName}",
					"asset",
					"Please enter a file name to save the glyph asset to",
					 _glyphAssetPath
				);

				if ( !string.IsNullOrEmpty( savePath ) )
				{
					var newGlyphAsset = ScriptableObject.CreateInstance<ControllerGlyphs>();
					newGlyphAsset.Construct(
						EditorControllerName,
						ControllerName,
						ControllerGuid,
						Elements.Select( e => (e.ElementId, e.ElementIdName) ).ToList()
					);

					AssetDatabase.CreateAsset( newGlyphAsset, savePath );
					AssetDatabase.Refresh();

					EditorUtility.FocusProjectWindow();
					EditorGUIUtility.PingObject( newGlyphAsset );
				}
			}
		}
	}
}