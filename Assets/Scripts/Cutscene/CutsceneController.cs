using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Cutscene
{
	public class CutsceneController : IInitializable,
		ITickable
    {
		private readonly Settings _settings;
		private readonly CutsceneModel _model;
		private readonly PlayerInputResolver _inputResolver;

		private Rewired.Player _input;
		private float _recentButtonExpirationTime;

		public CutsceneController( Settings settings,
			CutsceneModel model,
            PlayerInputResolver inputResolver )
		{
			_settings = settings;
			_model = model;
			_inputResolver = inputResolver;
		}

		public void Initialize()
		{
			_input = _inputResolver.GetInput();
		}

		public void Tick()
		{
			if ( _model.IsPlaying )
			{
				if ( _input.GetAnyButton() )
				{
					_recentButtonExpirationTime = Time.timeSinceLevelLoad + _settings.RecentButtonQueryDuration;
					_model.SetRecentButtonPressedState( true );
				}
				else if ( _recentButtonExpirationTime < Time.timeSinceLevelLoad )
				{
					_recentButtonExpirationTime = 0;
					_model.SetRecentButtonPressedState( false );
				}

				if ( _input.GetButtonTimedPressDown( ReConsts.Action.Fire, _settings.HoldSkipDuration ) )
				{
					_model.SetPlayState( false );
				}
			}
		}

		[System.Serializable]
		public class Settings
		{
			public float HoldSkipDuration = 1;
			public float RecentButtonQueryDuration = 1;
		}
	}
}
