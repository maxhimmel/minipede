using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class CleansedAreaAnimator
	{
		private readonly Settings _settings;
		private readonly SpriteRenderer[] _renderers;
		private readonly MaterialPropertyBlock _matPropBlock;

		private float _maxExtents;

		public CleansedAreaAnimator( Settings settings,
			SpriteRenderer[] renderers )
		{
			_settings = settings;
			_renderers = renderers;

			_matPropBlock = new MaterialPropertyBlock();

			InitShaderProperties();
		}

		private void InitShaderProperties()
		{
			Bounds combinedBounds = _renderers[0].bounds;
			Vector3 center = combinedBounds.center;
			for ( int idx = 1; idx < _renderers.Length; ++idx )
			{
				var bounds = _renderers[idx].bounds;

				center += bounds.center;
				combinedBounds.Encapsulate( bounds );
			}
			center /= _renderers.Length;

			_matPropBlock.SetVector( _settings.CenterProperty, center );
			_matPropBlock.SetFloat( _settings.RadiusProperty, 0 );

			UpdateRendererPropertyBlock();

			var combinedExtents = combinedBounds.extents;
			_maxExtents = _settings.RadiusPadding + Mathf.Max( combinedExtents.x, combinedExtents.y, combinedExtents.z );
		}

		public async UniTaskVoid Play( CancellationToken cancelToken )
		{
			float timer = 0;
			while ( timer < _settings.Duration )
			{
				timer += Time.deltaTime;
				float radiusTween = _settings.GrowCurve.Evaluate( timer / _settings.Duration );

				float radius = radiusTween * _maxExtents;
				_matPropBlock.SetFloat( _settings.RadiusProperty, radius );
				UpdateRendererPropertyBlock();

				await UniTask.Yield( PlayerLoopTiming.Update, cancelToken );
			}

			ImmediateFillCleansedArea();
		}

		private void UpdateRendererPropertyBlock()
		{
			foreach ( var renderer in _renderers )
			{
				renderer.SetPropertyBlock( _matPropBlock );
			}
		}

		public void ImmediateFillCleansedArea()
		{
			_matPropBlock.SetFloat( _settings.RadiusProperty, _maxExtents );
			UpdateRendererPropertyBlock();
		}

		[System.Serializable]
		public class Settings
		{
			[BoxGroup( "Properties", ShowLabel = false )]
			public string CenterProperty = "_Center";
			[BoxGroup( "Properties", ShowLabel = false )]
			public string RadiusProperty = "_MaskRadius";

			[BoxGroup( "Animation", ShowLabel = false )]
			public float Duration = 1;
			[BoxGroup( "Animation", ShowLabel = false )]
			public AnimationCurve GrowCurve = AnimationCurve.EaseInOut( 0, 0, 1, 1 );
			[BoxGroup( "Animation", ShowLabel = false )]
			public float RadiusPadding = 1;
		}
	}
}