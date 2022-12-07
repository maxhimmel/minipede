namespace Minipede.Utility
{
	public class Scalar
	{
		public float Scale { get; private set; } = 1;

		public void SetScale( float newScale )
		{
			Scale = newScale;
		}
	}
}