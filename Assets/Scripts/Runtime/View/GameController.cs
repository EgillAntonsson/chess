using System;
using System.Collections.Generic;
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
				var validMoves = tile switch
				{
					TileWithPiece twp when twp.Piece.PlayerId == game.PlayerIdToMove => game.FindValidMoves(twp),
					_ => Array.Empty<Position>()
					// TODO: highlight valid moves Piece tiles
				};
			}
		}
	}
}