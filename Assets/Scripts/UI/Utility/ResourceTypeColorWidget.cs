using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class ResourceTypeColorWidget : MonoBehaviour
	{
		[SerializeField] private MonoColorWidget _colorWidget;

		private ResourceType _resource;

		[Inject]
		public void Construct( [InjectOptional] ResourceType resource )
		{
			SetResource( resource );
		}

		public void SetResource( ResourceType resource )
		{
			_resource = resource;

			var color = resource != null
				? resource.Color
				: Color.clear;

			_colorWidget.SetColor( color );
		}
	}
}