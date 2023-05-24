using Minipede.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class SetSelected : MonoBehaviour
	{
		private ControllerModel _controllerModel;

		[Inject]
		public void Construct( ControllerModel controllerModel )
		{
			_controllerModel = controllerModel;
		}

		private void OnEnable()
		{
			_controllerModel.Changed += OnControllerChanged;

			OnControllerChanged( _controllerModel );
		}

		private void OnDisable()
		{
			_controllerModel.Changed -= OnControllerChanged;
		}

		private void OnControllerChanged( ControllerModel model )
		{
			var controlType = model.ControllerType;
			if ( controlType == Rewired.ControllerType.Joystick )
			{
				EventSystem.current.SetSelectedGameObject( gameObject );
			}
			else
			{
				EventSystem.current.SetSelectedGameObject( null );
			}
		}
	}
}