using System;
using System.Threading;
using Minipede.Gameplay.Player;

namespace Minipede.Gameplay
{
	public class SceneRunningHandler : IPlayerLifetimeHandler,
		IDisposable
	{
		public CancellationToken PlayerDiedCancelToken => _cancelSource.Token;

		private readonly CancellationTokenSource _cancelSource;

		public SceneRunningHandler()
		{
			_cancelSource = AppHelper.CreateLinkedCTS();
		}

		public void Dispose()
		{
			_cancelSource.Cancel();
			_cancelSource.Dispose();
		}
	}
}