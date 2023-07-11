using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Minimap;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.UI;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class ShipShrapnel : Collectable<ShipShrapnel>,
		IMapMarker
	{
		public Transform Avatar => _body.transform;
		public MinimapMarker MarkerPrefab => _markerPrefab;

		private Settings _shrapnelSettings;
		private MinimapModel _minimap;
		private MinimapMarker _markerPrefab;
		private TargetGroupAttachment _targetGroupAttachment;

		private float _cameraFocusEndTime;

		[Inject]
		public void Construct( Settings settings,
			MinimapModel minimap,
			MinimapMarker markerPrefab,
			TargetGroupAttachment targetGroupAttachment )
		{
			_shrapnelSettings = settings;
			_minimap = minimap;
			_markerPrefab = markerPrefab;
			_targetGroupAttachment = targetGroupAttachment;

			_cameraFocusEndTime = Mathf.Infinity;
		}

		public override void Launch( Vector2 impulse )
		{
			base.Launch( impulse );

			_minimap.AddMarker( this );

			_cameraFocusEndTime = Time.timeSinceLevelLoad + _shrapnelSettings.CameraFocusDuration;
		}

		protected override void HandleDisposal()
		{
			_cameraFocusEndTime = Mathf.Infinity;
			DeactivateCameraFocus();

			_minimap.RemoveMarker( this );

			base.HandleDisposal();
		}

		public override void FixedTick()
		{
			base.FixedTick();

			if ( !_isDisposed && _cameraFocusEndTime <= Time.timeSinceLevelLoad )
			{
				DeactivateCameraFocus();
			}
		}

		private void DeactivateCameraFocus()
		{
			if ( _targetGroupAttachment.State == TargetGroupAttachment.States.Active )
			{
				_targetGroupAttachment.transform.SetParent( null );
				_targetGroupAttachment.Deactivate( canDispose: true ).Forget();
			}
		}

		protected override ShipShrapnel GetCollectable()
		{
			return this;
		}

		[System.Serializable]
		public new class Settings
		{
			[MinValue( 0 )]
			public float CameraFocusDuration = 1;
		}

		public class Factory : UnityFactory<ShipShrapnel>
		{
			public Factory( DiContainer container, ShipShrapnel prefab ) 
				: base( container, prefab )
			{
			}
		}
	}
}
