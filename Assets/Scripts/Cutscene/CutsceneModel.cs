namespace Minipede.Gameplay.Cutscene
{
	public class CutsceneModel
    {
        public event System.Action<CutsceneModel> PlayStateChanged;
        public event System.Action<CutsceneModel> AnyButtonPressed;

        public bool IsPlaying { get; private set; }
        public bool IsButtonRecentlyPressed { get; private set; }

        public void SetPlayState( bool isPlaying )
		{
            if ( IsPlaying != isPlaying )
            {
                IsPlaying = isPlaying;
                PlayStateChanged?.Invoke( this );

                if ( !isPlaying )
				{
                    SetRecentButtonPressedState( false );
				}
            }
		}

        public void SetRecentButtonPressedState( bool isRecentlyPressed )
        {
            if ( IsButtonRecentlyPressed != isRecentlyPressed )
            {
                IsButtonRecentlyPressed = isRecentlyPressed;

                AnyButtonPressed?.Invoke( this );
            }
        }
    }
}
