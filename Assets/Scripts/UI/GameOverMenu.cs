using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GameOverMenu : MonoBehaviour
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;

		private SceneLoader _sceneLoader;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( SceneLoader sceneLoader,
			SignalBus signalBus )
		{
			_sceneLoader = sceneLoader;
			_signalBus = signalBus;

			_signalBus.Subscribe<EjectStateChangedSignal>( OnEjectStateChanged );
			_restartButton.onClick.AddListener( OnRestart );
			_exitButton.onClick.AddListener( OnExit );
		}

		private void OnEjectStateChanged( EjectStateChangedSignal signal )
		{
			var choice = signal.Model.Choice;
			if ( choice.HasValue && choice == EjectModel.Options.Die )
			{
				_signalBus.Unsubscribe<EjectStateChangedSignal>( OnEjectStateChanged );
				gameObject.SetActive( true );
			}
		}

		private void OnRestart()
		{
			var currentScene = _sceneLoader.Current;
			_sceneLoader.Load( currentScene.name ).Forget();
		}

		private void OnExit()
		{
			_sceneLoader.Load( "StartMenu" ).Forget();
		}
	}
}
