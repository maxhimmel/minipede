namespace Minipede.Gameplay.Cutscene
{
	public class CutsceneModel
    {
        public event System.Action<CutsceneModel> PlayStateChanged;

        public bool IsPlaying { get; private set; }

        public void SetPlayState( bool isPlaying )
		{
            if ( IsPlaying != isPlaying )
            {
                IsPlaying = isPlaying;

                PlayStateChanged?.Invoke( this );
            }
		}
    }
}
