using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class WaveController
	{
		public event System.Action<IWave, IWave.Result> Completed;

		public bool IsRunning => !IsEmpty && CurrentWave.IsRunning;
		public bool IsEmpty => _waveQueue.Count <= 0;
		private IWave CurrentWave => _waveQueue[CurrentWaveIndex];
		public int MinFillSize => CurrentWaveIndex + _settings.QueueSize;
		public int CurrentWaveIndex { get; private set; }

		private readonly Settings _settings;
		private readonly IPlayerLifetimeHandler _playerController;
		private readonly SignalBus _signalBus;
		private readonly SortedList<int, IWave> _waveQueue;

		public WaveController( Settings settings,
			IPlayerLifetimeHandler playerController,
			SignalBus signalBus )
		{
			_settings = settings;
			_playerController = playerController;
			_signalBus = signalBus;

			_waveQueue = new SortedList<int, IWave>();
		}

		public virtual void Insert( int index, IWave newWave )
		{
			if ( !_waveQueue.TryAdd( index, newWave ) )
			{
				var queueIndices = _waveQueue.Keys;
				int endIdx = _waveQueue.IndexOfKey( index );

				for ( int idx = queueIndices.Count - 1; idx >= endIdx; --idx )
				{
					int queueIdx = queueIndices[idx];

					_waveQueue.Remove( queueIdx, out var shiftedWave );
					_waveQueue.Add( ++queueIdx, shiftedWave );
				}

				_waveQueue.Add( index, newWave );
			}

			if ( _waveQueue.Count >= _settings.QueueSize )
			{
				_signalBus.Fire( new WaveQueueChangedSignal()
				{
					CurrentWave = CurrentWaveIndex,
					IdQueue = GetWaveIdRange( CurrentWaveIndex, _settings.QueueSize )
				} );
			}
		}

		private List<string> GetWaveIdRange( int start, int count )
		{
			var result = new List<string>( count );

			for ( int idx = start; idx < start + count; ++idx )
			{
				result.Add( _waveQueue[idx].Id );
			}

			return result;
		}

		public async virtual UniTask Play()
		{
			if ( IsEmpty )
			{
				throw new System.NotImplementedException( "Queue is empty." );
			}
			if ( IsRunning )
			{
				throw new System.NotImplementedException( "Wave is already running." );
			}

			_signalBus.Fire( new WaveProgressSignal()
			{
				Id = CurrentWave.Id,
				NormalizedProgress = 0
			} );

			await TaskHelpers.DelaySeconds( _settings.StartDelay, _playerController.PlayerDiedCancelToken );

			var result = await CurrentWave.Play();

			if ( result != IWave.Result.Restart )
			{
				CompleteWave( CurrentWave, result );
			}

			if ( result == IWave.Result.Success && !IsEmpty )
			{
				Play().Forget();
			}
		}

		private void CompleteWave( IWave wave, IWave.Result result )
		{
			_waveQueue.Remove( CurrentWaveIndex++ );

			Completed?.Invoke( wave, result );
		}

		public virtual void Interrupt()
		{
			if ( !IsRunning )
			{
				return;
			}

			CurrentWave.Interrupt();
		}

		public int GetNextAvailableIndex()
		{
			for ( int idx = CurrentWaveIndex; idx <= MinFillSize; ++idx )
			{
				if ( !_waveQueue.ContainsKey( idx ) )
				{
					return idx;
				}
			}

			return MinFillSize + 1;
		}

		[System.Serializable]
		public struct Settings
		{
			public int QueueSize;
			public float StartDelay;
		}
	}
}