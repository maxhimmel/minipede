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
    }
}
