using UnityEngine;

namespace Minipede
{
	public static class AppHelper
	{
		public static bool IsQuitting => _isQuitting || !Application.isPlaying;

		private static bool _isQuitting = false;

		[RuntimeInitializeOnLoadMethod]
		public static void ListenForQuitRequest()
		{
			Debug.Log( $"Listening for quit request." );

			_isQuitting = false;
			Application.quitting += OnAppQuitting;
		}

		private static void OnAppQuitting()
		{
			Debug.Log( $"App is quitting." );

			_isQuitting = true;
			Application.quitting -= OnAppQuitting;
		}
	}
}
