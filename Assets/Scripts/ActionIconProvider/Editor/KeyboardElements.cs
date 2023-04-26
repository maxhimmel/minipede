﻿using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
	[HideReferenceObjectPicker]
	[System.Serializable]
	public class KeyboardElements
	{
		private const string _glyphAssetPath = "Assets/Scripts/ActionIconProvider/Assets";

		[ListDrawerSettings( IsReadOnly = true, ShowPaging = false )]
		public List<KeyboardElementId> Elements;

		[OnInspectorGUI, PropertyOrder( -1 )]
		private void CreateGlyphAsset()
		{
			if ( SirenixEditorGUI.MenuButton( EditorGUI.indentLevel, " Create Glyph Asset", false, EditorIcons.ImageCollection.Raw ) )
			{
				var savePath = EditorUtility.SaveFilePanelInProject(
					"Save Keyboard Glyph Asset",
					"Keyboard",
					"asset",
					"Please enter a file name to save the glyph asset to",
					 _glyphAssetPath
				);

				if ( !string.IsNullOrEmpty( savePath ) )
				{
					var newGlyphAsset = ScriptableObject.CreateInstance<KeyboardGlyphs>();
					newGlyphAsset.Construct(
						Elements.Select( e => (e.Element.Id, e.Element.Name, e.Element.Type) ).ToList()
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