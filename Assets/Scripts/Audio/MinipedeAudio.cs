using Minipede.Gameplay.Player;
using UnityEngine;

namespace Minipede.Gameplay.Audio
{
	public class MinipedeAudio : IMinipedeAudio
	{
		private readonly IAudioController _audioController;
		private readonly PlayerController _playerController;

		public MinipedeAudio( IAudioController audioController,
			PlayerController playerController )
		{
			_audioController = audioController;
			_playerController = playerController;
		}

		public void PlayOneShot( string key, Vector2 position )
		{
			//if ( !_playerController.IsExploring )
			//{
			//	// Snap all audio positions to the ship to simulate non-3D audio ...
			//	position = _playerController.GetOrientation().Position;
			//}

			_audioController.PlayOneShot( key, position );
		}
	}
}