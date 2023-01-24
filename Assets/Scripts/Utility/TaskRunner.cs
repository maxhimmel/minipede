using System.Threading;
using Cysharp.Threading.Tasks;

namespace Minipede.Utility
{
	public class TaskRunner
	{
		public bool IsRunning { get; private set; }
		public CancellationToken CancelToken => _cancelSource.Token;

		private CancellationTokenSource _cancelSource;

		public TaskRunner()
		{
			_cancelSource = AppHelper.CreateLinkedCTS();
		}
		public TaskRunner( CancellationToken cancelToken )
		{
			_cancelSource = AppHelper.CreateLinkedCTS( cancelToken );
		}

		public async UniTask Run( System.Func<UniTask> task )
		{
			TryCancel();

			IsRunning = true;

			await task
				.Invoke()
				.Cancellable( _cancelSource.Token );

			IsRunning = false;
		}

		public bool TryCancel()
		{
			if ( !IsRunning )
			{
				return false;
			}

			_cancelSource.Cancel();
			_cancelSource.Dispose();
			_cancelSource = AppHelper.CreateLinkedCTS();

			return true;
		}
	}
}