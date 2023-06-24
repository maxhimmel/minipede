using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Cutscene;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class CutsceneWidget : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _skipPromptFader;
		[SerializeField] private float _skipPromptFadeDuration = 0.3f;

		private CutsceneModel _model;
		private TaskRunner _fadeRunner;

		[Inject]
		public void Construct( CutsceneModel model )
		{
			_model = model;
		}

		private void OnDestroy()
		{
			_model.AnyButtonPressed -= OnInputReceived;
		}

		private void Awake()
		{
			_model.AnyButtonPressed += OnInputReceived;

			_fadeRunner = new TaskRunner( this.GetCancellationTokenOnDestroy() );
			_skipPromptFader.alpha = 0;
		}

		private void OnInputReceived( CutsceneModel model )
		{
			if ( model.IsButtonRecentlyPressed )
			{
				_fadeRunner.Run( () => HandleSkipPromptFading( 0, 1 ) ).Forget();
			}
			else
			{
				_fadeRunner.Run( () => HandleSkipPromptFading( 1, 0 ) ).Forget();
			}
		}

		private async UniTask HandleSkipPromptFading( float startAlpha, float endAlpha )
		{
			float fadeTimer = 0;
			while ( fadeTimer < _skipPromptFadeDuration )
			{
				fadeTimer += Time.deltaTime;
				_skipPromptFader.alpha = Mathf.Lerp( startAlpha, endAlpha, fadeTimer / _skipPromptFadeDuration );

				await UniTask.Yield( PlayerLoopTiming.Update, _fadeRunner.CancelToken );
			}
		}
	}
}