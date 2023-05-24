using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
    public class ControlMapAutoToggler : MonoBehaviour
    {
		public static bool CanEnable { get; set; } = true;
		public static bool CanDisable { get; set; } = true;

		[SerializeField] private string _controlMapName;
		[SerializeField] private bool _isEnabled;

		private PlayerInputResolver _inputResolver;

		[Inject]
		public void Construct( PlayerInputResolver inputResolver )
		{
			_inputResolver = inputResolver;
		}

		private void OnEnable()
		{
			if ( CanEnable )
			{
				var input = _inputResolver.GetInput();
				input.EnableMapRuleSet( _controlMapName, _isEnabled );
			}
		}

		private void OnDisable()
		{
			if ( AppHelper.IsQuitting )
			{
				// Prevent exception being thrown by Rewired framework.
				return;
			}

			if ( CanDisable )
			{
				var input = _inputResolver.GetInput();
				input.EnableMapRuleSet( _controlMapName, !_isEnabled );
			}
		}
	}
}
