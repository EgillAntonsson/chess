using Chess;

public class Player
{
	public int PlayerId { get; }
	public CheckType IsInCheckType { get; }
	internal TileWithPiece LastMovedTilePiece;

	public Player(int playerId, CheckType isInCheckType)
	{
		PlayerId = playerId;
		IsInCheckType = isInCheckType;
	}
}