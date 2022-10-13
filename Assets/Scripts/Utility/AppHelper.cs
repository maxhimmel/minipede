using UnityEngine;

namespace Minipede
{
	public static class AppHelper
	{
		public static bool IsQuitting { get; private set; }

		[RuntimeInitializeOnLoadMethod]
		public static void ListenForQuitRequest()
		{
			Debug.Log( $"Listening for quit request." );

			IsQuitting = false;
			Application.quitting += OnAppQuitting;
		}

		private static void OnAppQuitting()
		{
			Debug.Log( $"App is quitting." );

			IsQuitting = true;
			Application.quitting -= OnAppQuitting;
		}
	}
}
