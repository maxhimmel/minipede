using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Minipede.Utility
{
    public static class TaskHelpers
    {
        public static UniTask DelaySeconds( float seconds )
		{
            if ( seconds <= 0 )
			{
                return UniTask.CompletedTask;
			}

            return UniTask.Delay( TimeSpan.FromSeconds( seconds ) );
		}

        public static UniTask Cancellable( this UniTask task, CancellationToken token)
		{
			return task.AttachExternalCancellation( token );
		}
	}
}
