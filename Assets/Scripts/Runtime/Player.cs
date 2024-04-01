using Chess;

public class Player
{
	public int PlayerId { get; }
	public CheckType IsInCheckType { get; }

	public bool IsInCheck => IsInCheckType == CheckType.Check;
	internal TileWithPiece LastMovedTilePiece;

	public Player(int playerId, CheckType isInCheckType = CheckType.NoCheck)
	{
		PlayerId = playerId;
		IsInCheckType = isInCheckType;
	}
}