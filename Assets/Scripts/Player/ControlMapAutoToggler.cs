using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
    public class ControlMapAutoToggler : MonoBehaviour
    {
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
			var input = _inputResolver.GetInput();
			input.EnableMapRuleSet( _controlMapName, _isEnabled );
		}

		private void OnDisable()
		{
			var input = _inputResolver.GetInput();
			input.EnableMapRuleSet( _controlMapName, !_isEnabled );
		}
	}
}
