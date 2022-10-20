using System.Threading;
using UnityEngine;

namespace Minipede
{
	public static class AppHelper
	{
		public static bool IsQuitting => _isQuitting || !Application.isPlaying;
		public static CancellationToken AppQuittingToken { get; private set; }

		private static bool _isQuitting = false;
		private static CancellationTokenSource _cancellationSource;

		[RuntimeInitializeOnLoadMethod]
		public static void ListenForQuitRequest()
		{
			_isQuitting = false;
			_cancellationSource = new CancellationTokenSource();
			AppQuittingToken = _cancellationSource.Token;

			Application.quitting += OnAppQuitting;
		}

		private static void OnAppQuitting()
		{
			_isQuitting = true;
			_cancellationSource.Cancel();

			Application.quitting -= OnAppQuitting;
		}
	}
}
