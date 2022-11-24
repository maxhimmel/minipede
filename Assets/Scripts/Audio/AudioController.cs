using System.Collections.Generic;
using UnityEngine;

namespace Minipede.Gameplay.Audio
{
    public class AudioController : MonoBehaviour,
		IAudioController
    {
        [SerializeField] private AudioBank[] _banks;

        private readonly Dictionary<string, BankEvent> _events = new Dictionary<string, BankEvent>();

		private void Awake()
		{
			foreach ( var bank in _banks )
			{
				foreach ( var data in bank.Events )
				{
					string key = bank.ExportKey( data );
					_events.Add( key, data );
				}
			}
		}

		public void PlayOneShot( string key, Vector2 position )
		{
			if ( !_events.TryGetValue( key, out var data ) )
			{
				throw new KeyNotFoundException( key );
			}

			AudioSource.PlayClipAtPoint( data.Clip, position );
		}
	}
}
