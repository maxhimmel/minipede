using Rewired;

namespace Minipede.Utility
{
	public static class RewiredExtensions
	{
		public static void AddButtonPressedDelegate( this Player input,
			System.Action<InputActionEventData> callback,
			int actionId,
			InputActionEventType eventType = InputActionEventType.ButtonJustPressed,
			UpdateLoopType updateLoop = UpdateLoopType.Update )
		{
			input.AddInputEventDelegate( callback, eventType, actionId, updateLoop );
		}

		public static void AddButtonReleasedDelegate( this Player input,
			System.Action<InputActionEventData> callback,
			int actionId,
			InputActionEventType eventType = InputActionEventType.ButtonJustReleased,
			UpdateLoopType updateLoop = UpdateLoopType.Update )
		{
			input.AddInputEventDelegate( callback, eventType, actionId, updateLoop );
		}

		public static void AddAxisDelegate( this Player input,
			System.Action<InputActionEventData> callback,
			int actionId,
			InputActionEventType eventType = InputActionEventType.AxisActiveOrJustInactive,
			UpdateLoopType updateLoop = UpdateLoopType.Update )
		{
			input.AddInputEventDelegate( callback, eventType, actionId, updateLoop );
		}

		public static void AddInputEventDelegate( this Player input, 
			System.Action<InputActionEventData> callback,
			InputActionEventType eventType,
			int actionId,
			UpdateLoopType updateLoop = UpdateLoopType.Update )
		{
			input.AddInputEventDelegate( 
				callback,
				updateLoop, 
				eventType,
				actionId 
			);
		}
	}
}