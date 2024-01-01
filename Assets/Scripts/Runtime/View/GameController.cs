using System;
using UnityEngine;

namespace Chess.View
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private ChessBoardView chessBoardView;

		private Game game;
		private bool playerHasSelectedPiece;

		private void Start()
		{
			game = new Game(VariantFactory.Create(VariantType.Standard));
			chessBoardView.Create(game.ChessBoard, OnTileClicked);
		}

		private void OnTileClicked(Tile tile)
		{
			Debug.Log(tile);
			if (!playerHasSelectedPiece)
			{
				if (!tile.HasPiece)
				{
					// TODO: tell view tiles to de-highlight
					return;
				}
				if (game.PlayerIdToMove == tile.Piece.PlayerId)
				{
					// TODO: highlight piece  tile
					var validMoves = game.FindValidMoves(tile);
				}
			}
		}
	}
}