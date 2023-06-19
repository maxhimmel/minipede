using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class CleansedAreaAnimator
	{
		private const float _radiusBuffer = 1;

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
			Vector3 center = Vector3.zero;
			Bounds combinedBounds = new Bounds();
			foreach ( var renderer in _renderers )
			{
				var bounds = renderer.bounds;

				center += bounds.center;
				combinedBounds.Encapsulate( bounds );
			}
			center /= _renderers.Length;

			_matPropBlock.SetVector( _settings.CenterProperty, center );
			_matPropBlock.SetFloat( _settings.RadiusProperty, 0 );

			UpdateRendererPropertyBlock();

			var combinedExtents = combinedBounds.extents;
			_maxExtents = _radiusBuffer + Mathf.Max( combinedExtents.x, combinedExtents.y, combinedExtents.z );
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

			_matPropBlock.SetFloat( _settings.RadiusProperty, _maxExtents );
			UpdateRendererPropertyBlock();
		}

		private void UpdateRendererPropertyBlock()
		{
			foreach ( var renderer in _renderers )
			{
				renderer.SetPropertyBlock( _matPropBlock );
			}
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
		}
	}
}