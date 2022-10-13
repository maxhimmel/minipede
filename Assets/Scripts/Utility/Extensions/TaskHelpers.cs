using System;
using System.Threading.Tasks;

namespace Minipede.Utility
{
    public static class TaskHelpers
    {
        public static Task DelaySeconds( float seconds )
		{
            if ( seconds <= 0 )
			{
                return Task.CompletedTask;
			}

            return Task.Delay( TimeSpan.FromSeconds( seconds ) );
		}
    }
}
