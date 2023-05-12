using Rewired;
using Zenject;

namespace Minipede.Utility
{
	public class ControllerPoller : IInitializable,
		ITickable
	{
		private readonly ControllerModel _model;
		private readonly PlayerInputResolver _inputResolver;

        private Player _input;

		public ControllerPoller( ControllerModel model,
            PlayerInputResolver inputResolver )
		{
			_model = model;
			_inputResolver = inputResolver;
		}

		public void Initialize()
		{
            _input = _inputResolver.GetInput();
		}

		public void Tick()
        {
            if ( _input.controllers.joystickCount > 0 )
            {
                if ( TryUpdateControllerType( ControllerType.Joystick ) )
				{
                    return;
				}
            }
            if ( _input.controllers.hasKeyboard )
            {
                if ( TryUpdateControllerType( ControllerType.Keyboard ) )
                {
                    return;
                }
            }
            if ( _input.controllers.hasMouse )
            {
                if ( TryUpdateControllerType( ControllerType.Mouse ) )
                {
                    return;
                }
            }
        }

        private bool TryUpdateControllerType( ControllerType type )
		{
            if ( !CanPollController( type ) )
			{
                return false;
			}

            var pollInfo = _input.controllers.polling.PollAllControllersOfTypeForFirstElement( type );
            if ( pollInfo.success )
            {
                _model.SetControllerType( pollInfo.controllerType );
                return true;
            }

            return false;
        }

        private bool CanPollController( ControllerType type )
		{
            return _model.ControllerType != type;
		}
	}
}