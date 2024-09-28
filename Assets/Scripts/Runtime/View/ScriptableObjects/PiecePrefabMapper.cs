using UnityEngine;

namespace Chess.View
{
	[CreateAssetMenu(fileName = "PiecePrefabMapper", menuName = "Chess/ScriptableObjects/PiecePrefabMapper", order = 1)]
	public class PiecePrefabMapper : ScriptableObject
	{
		[SerializeField] private PieceType[] pieceTypes;
		[SerializeField] private GameObject[] piecePrefabsPlayer1;
		[SerializeField] private GameObject[] piecePrefabsPlayer2;

		public (bool, GameObject) TryGetPiecePrefab(Tile tile)
		{
			return tile switch
			{
				TileWithPiece twp => TryGetPiecePrefab(twp),
				_ => (false, null)
			};
		}

		private (bool, GameObject) TryGetPiecePrefab(TileWithPiece tileWithPiece)
		{
			var prefabsForPlayer = tileWithPiece.Piece.PlayerId == 1 ? piecePrefabsPlayer1 : piecePrefabsPlayer2;
			for (var i = 0; i < pieceTypes.Length; i++)
			{
				if (pieceTypes[i] == tileWithPiece.Piece.Type)
				{
					return (true, prefabsForPlayer[i]);
				}
			}

			return (false, null);
		}
	}
}