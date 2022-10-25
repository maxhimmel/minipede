using UnityEngine;
using UnityEditor;

namespace Minipede.Editor
{
    public static class EditorUtility
    {
        public static TAsset FindAsset<TAsset>( string assetName )
			where TAsset : Object
		{
			string[] guids = AssetDatabase.FindAssets( assetName );
			string path = AssetDatabase.GUIDToAssetPath( guids[0] );
			return AssetDatabase.LoadAssetAtPath<TAsset>( path );
		}

		public static void SaveToEditorPref( string key, object saveMe )
		{
			EditorPrefs.SetString( key, JsonUtility.ToJson( saveMe, prettyPrint: false ) );
		}

		public static void LoadFromEditorPref( string key, object loadMe )
		{
			var json = EditorPrefs.GetString( key );
			JsonUtility.FromJsonOverwrite( json, loadMe );
		}
    }
}
