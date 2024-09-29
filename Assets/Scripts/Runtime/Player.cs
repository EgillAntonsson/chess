namespace Chess
{
	public class Player
	{
		public int Id { get; }
		public CheckType IsInCheckType { get; }
		public TileWithPiece LastMovedTilePiece { get; internal set; }

		public Player(int id, CheckType isInCheckType = CheckType.NoCheck)
		{
			Id = id;
			IsInCheckType = isInCheckType;
		}
	}
}