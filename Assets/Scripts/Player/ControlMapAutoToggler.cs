using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
    public class ControlMapAutoToggler : MonoBehaviour
    {
		[SerializeField] private string _controlMapName;
		[SerializeField] private bool _isEnabled;

		private Rewired.Player _input;

		[Inject]
		public void Construct( Rewired.Player input )
		{
            _input = input;
		}

		private void OnEnable()
		{
			_input.EnableMapRuleSet( _controlMapName, _isEnabled );
		}

		private void OnDisable()
		{
			_input.EnableMapRuleSet( _controlMapName, !_isEnabled );
		}
	}
}
