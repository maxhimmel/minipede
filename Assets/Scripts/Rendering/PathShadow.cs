using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Minipede.Rendering
{
    public class PathShadow : ShadowCaster2D
    {
        private static FieldInfo _shapeFieldInfo = typeof( ShadowCaster2D ).GetField( "m_ShapePath",
            BindingFlags.NonPublic | BindingFlags.Instance );

        private static FieldInfo _shapeHashFieldInfo = typeof( ShadowCaster2D ).GetField( "m_ShapePathHash",
            BindingFlags.NonPublic | BindingFlags.Instance );

        private static FieldInfo _sortingLayersFieldInfo = typeof( ShadowCaster2D ).GetField( "m_ApplyToSortingLayers",
            BindingFlags.NonPublic | BindingFlags.Instance );

        public void SetShape( List<Vector2> points, int[] sortingLayers )
        {
            Vector3[] shapev3 = points.ConvertAll( ( point ) => new Vector3( point.x, point.y ) ).ToArray();
            _shapeFieldInfo.SetValue( this, shapev3 );
            _sortingLayersFieldInfo.SetValue( this, sortingLayers );
            _shapeHashFieldInfo.SetValue( this, (int)Random.Range( 0f, 10000000f ) );
        }
    }
}
