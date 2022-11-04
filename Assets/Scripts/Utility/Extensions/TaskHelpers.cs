using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Minipede.Utility
{
    public static class TaskHelpers
    {
        public static UniTask DelaySeconds( float seconds, CancellationToken cancellationToken = default )
		{
            if ( seconds <= 0 )
			{
                return UniTask.CompletedTask;
			}

            return UniTask.Delay( TimeSpan.FromSeconds( seconds ), cancellationToken: cancellationToken );
		}

		public static UniTask WaitForFixedUpdate( CancellationToken cancellationToken = default )
		{
			if ( cancellationToken == default )
			{
				cancellationToken = AppHelper.AppQuittingToken;
			}
			return UniTask.WaitForFixedUpdate( cancellationToken );
		}

        public static UniTask Cancellable( this UniTask task, CancellationToken token, bool suppressCancellationThrow = true )
		{
			var result = task.AttachExternalCancellation( token );
			if ( suppressCancellationThrow )
			{
				result = result.SuppressCancellationThrow();
			}

			return result;
		}
	}
}
