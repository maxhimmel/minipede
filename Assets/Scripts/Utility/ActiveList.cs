using System;
using System.Collections.Generic;
using Zenject;

namespace Minipede.Utility
{
	public class ActiveList<TActive, TAddSignal, TRemoveSignal> :
		IInitializable,
		IDisposable
		where TActive : IDisposable
		where TAddSignal : IValueSignal<TActive>
		where TRemoveSignal : IValueSignal<TActive>
	{
		public IReadOnlyList<TActive> Actives => _actives;

		private readonly List<TActive> _actives;
		private readonly SignalBus _signalBus;

		private bool _isClearing;

		public ActiveList( SignalBus signalBus )
		{
			_actives = new List<TActive>();
			_signalBus = signalBus;
		}

		public void Initialize()
		{
			_signalBus.Subscribe<TAddSignal>( OnAdded );
			_signalBus.Subscribe<TRemoveSignal>( OnRemoved );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<TAddSignal>( OnAdded );
			_signalBus.Unsubscribe<TRemoveSignal>( OnRemoved );
		}

		private void OnAdded( TAddSignal signal )
		{
			_actives.Add( signal.Value );
		}

		private void OnRemoved( TRemoveSignal signal )
		{
			if ( !_isClearing )
			{
				_actives.Remove( signal.Value );
			}
		}

		public void Clear()
		{
			_isClearing = true;
			{
				for ( int idx = _actives.Count - 1; idx >= 0; --idx )
				{
					var value = _actives[idx];
					value.Dispose();

					_actives.RemoveAt( idx );
				}
			}
			_isClearing = false;
		}
	}
}