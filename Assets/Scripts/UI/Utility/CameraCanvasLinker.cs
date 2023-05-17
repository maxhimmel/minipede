using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class CameraCanvasLinker : IInitializable
    {
		private readonly Canvas _canvas;
		private readonly Camera _camera;

		public CameraCanvasLinker( Canvas canvas,
			Camera camera )
		{
			_canvas = canvas;
			_camera = camera;
		}

		public void Initialize()
		{
			_canvas.worldCamera = _camera;
		}
	}
}
