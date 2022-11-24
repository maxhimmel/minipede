using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

namespace Minipede.Gameplay.Audio
{
    public class AudioController : MonoBehaviour,
		IAudioController
    {
		[BoxGroup]
		[SerializeField] private AudioMixer _masterMixer;
		[BoxGroup]
		[SerializeField] private AudioSource[] _sourcePrefabs;

		[Space]
        [SerializeField] private AudioBank[] _banks;

        private readonly Dictionary<string, BankEvent> _events = new Dictionary<string, BankEvent>();
		private readonly Dictionary<string, ObjectPool<AudioSource>> _sourcesByMixer = new Dictionary<string, ObjectPool<AudioSource>>();
		private readonly List<AudioSource> _playingSources = new List<AudioSource>();

		private void Awake()
		{
			// Init event lookup ...
			foreach ( var bank in _banks )
			{
				foreach ( var data in bank.Events )
				{
					string key = bank.ExportKey( data );
					_events.Add( key, data );
				}
			}

			// Init audio source lookup ...
			foreach ( var source in _sourcePrefabs )
			{
				_sourcesByMixer.Add(
					source.outputAudioMixerGroup.name, new ObjectPool<AudioSource>(
						createFunc:			() => Instantiate( source, transform ),
						actionOnGet:		source => source.gameObject.SetActive( true ),
						actionOnRelease:	source => source.gameObject.SetActive( false ),
						actionOnDestroy:	source => Destroy( source.gameObject ),
						maxSize:			30
				) );
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

			var mixerKey = key.Split( '/' )[0];
			var sourcePool = _sourcesByMixer[mixerKey];
			var source = sourcePool.Get();

			// 3D ...
			source.spatialBlend = data.Is3d ? 1 : 0;
			source.minDistance = data.DistanceRange.x;
			source.maxDistance = data.DistanceRange.y;
			source.transform.position = position;

			// General ...
			source.volume = data.Volume;
			source.pitch = data.PitchRange.Random();

			// Play!
			source.clip = data.Clip;
			source.Play();

			_playingSources.Add( source );
		}

		private void Update()
		{
			for ( int idx = _playingSources.Count - 1; idx >= 0; --idx )
			{
				var source = _playingSources[idx];
				if ( !source.isPlaying )
				{
					string mixerKey = source.outputAudioMixerGroup.name;
					var sourcePool = _sourcesByMixer[mixerKey];

					sourcePool.Release( source );
					_playingSources.RemoveAt( idx );
				}
			}
		}

		private void OnDestroy()
		{
			foreach ( var kvp in _sourcesByMixer )
			{
				kvp.Value.Dispose();
			}
		}
	}
}
