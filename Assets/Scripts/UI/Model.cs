using Zenject;

namespace Minipede.Gameplay.UI
{
	public class Model<TSignal>
	{
		private readonly SignalBus _signalBus;

		private System.Action<TSignal> _callback;

		public Model( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		public void Listen( System.Action<TSignal> callback )
		{
			_callback = callback;
			_signalBus.Subscribe( _callback );
		}

		public void Cleanup()
		{
			_signalBus.Unsubscribe( _callback );
		}
	}
}