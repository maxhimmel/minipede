using Rewired;
using TMPro;
using UnityEngine;
using Zenject;

namespace ControllerGlyph.Samples
{
	public class ControllerGlyphTesting : MonoBehaviour
    {
		[SerializeField] private TextMeshPro _display;
		[SerializeField] private float _clearDelay = 2;
		[SerializeField] private ControllerType _controller;

		private ControllerGlyphBus _glyphBus;
		private Player _input;

		private float _nextClearTime;

        [Inject]
		public void Construct( ControllerGlyphBus glyphBus,
			Player input )
		{
			_glyphBus = glyphBus;
			_input = input;
		}

		private void Start()
		{
			_display.text = string.Empty;
			_input.AddInputEventDelegate( OnButtonReceived, UpdateLoopType.Update, InputActionEventType.ButtonJustPressed );
			_input.AddInputEventDelegate( OnAxisReceived, UpdateLoopType.Update, InputActionEventType.AxisActive );
		}

		private void OnButtonReceived( InputActionEventData data )
		{
			_nextClearTime = Time.timeSinceLevelLoad + _clearDelay;

			AxisRange axisRange = data.GetAxis() > 0
				? AxisRange.Positive
				: AxisRange.Negative;

			var request = new GlyphRequest( data.actionId )
			{
				Controller = _controller,
				AxisRange = axisRange,
				ElementType = ControllerElementType.Button
			};

			_display.text += $"{_glyphBus.GetGlyph( request )} : {data.actionName}\n";
		}

		private void OnAxisReceived( InputActionEventData data )
		{
			_nextClearTime = Time.timeSinceLevelLoad + _clearDelay;

			AxisRange axisRange = data.GetAxis() > 0
				? AxisRange.Positive
				: AxisRange.Negative;

			var request = new GlyphRequest( data.actionId )
			{
				Controller = _controller,
				AxisRange = axisRange,
				ElementType = ControllerElementType.Axis
			};

			_display.text += $"{_glyphBus.GetGlyph( request )} : {data.actionName}\n";
		}

		private void Update()
		{
			if ( _nextClearTime <= Time.timeSinceLevelLoad )
			{
				_display.text = string.Empty;
			}
		}

		private class GlyphRequest : ControllerGlyphBus.Request
		{
			private readonly int _actionId;

			public GlyphRequest( int actionId )
			{
				_actionId = actionId;
			}

			public override int GetActionId()
			{
				return _actionId;
			}
		}
	}
}
