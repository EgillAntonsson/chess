namespace Chess
{
	public class Player
	{
		public int Id { get; }
		public CheckType IsInCheckType { get; }

		public bool IsInCheck => IsInCheckType == CheckType.Check;
		internal TileWithPiece LastMovedTilePiece;

		public Player(int id, CheckType isInCheckType)
		{
			Id = id;
			IsInCheckType = isInCheckType;
		}
	}
}