namespace Minipede.Gameplay
{
	public struct DamageResult
	{
		public static readonly DamageResult Empty = new DamageResult();

		public int DamageTaken;
		public string FxEventName;
	}
}