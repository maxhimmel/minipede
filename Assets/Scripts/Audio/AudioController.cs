using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minipede.ReConsts;
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

		public UniTask LoadBank( string category )
		{
			var bank = Array.Find( _banks, b => b.Category == category );
			if ( bank == null )
			{
				throw new KeyNotFoundException( category );
			}

			foreach ( var data in bank.Events )
			{
				data.Clip.LoadAudioData();
			}

			return UniTask.CompletedTask;
		}

		public UniTask UnloadBank( string category )
		{
			var bank = Array.Find( _banks, b => b.Category == category );
			if ( bank == null )
			{
				throw new KeyNotFoundException( category );
			}

			foreach ( var data in bank.Events )
			{
				data.Clip.UnloadAudioData();
			}

			return UniTask.CompletedTask;
		}

		public void PlayOneShot( string key, Vector2 position )
		{
			if ( !_events.TryGetValue( key, out var data ) )
			{
				throw new KeyNotFoundException( key );
			}

			if ( data.Clip.loadState != AudioDataLoadState.Loaded )
			{
				Debug.LogWarning( $"Playing '<b>{key}</b>' clip before loading. This may cause lag.\n" +
					$"Try using '<b>{nameof(IAudioController)}.{nameof(LoadBank)}</b>' prior to this call.", data.Clip );
			}

			AudioSource.PlayClipAtPoint( data.Clip, position );
		}
	}
}
