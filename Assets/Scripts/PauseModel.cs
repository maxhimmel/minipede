namespace Minipede
{
	public class PauseModel
	{
		public event System.Action<PauseModel> Changed;

		public bool IsPaused { get; private set; }

		/// <returns>The new pause state.</returns>
		public bool Toggle()
		{
			var newState = !IsPaused;

			Set( newState );

			return newState;
		}

		public void Set( bool isPaused )
		{
			IsPaused = isPaused;

			Changed?.Invoke( this );
		}
	}
}