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
		private IEnumerable<Position> validMoves;
		private TileView tileViewLastClicked;
		

		private void Start()
		{
			game = new Game(VariantFactory.Create(VariantType.Standard));
			chessBoardView.Create(game.ChessBoard, OnTileClicked);
		}

		private void OnTileClicked(TileView tileView)
		{
			Debug.Log(tileView.Tile);
			if (!playerHasSelectedPiece)
			{
				playerHasSelectedPiece = true;
				tileView.MarkAsSelected(doMark: true);
				validMoves = tileView.Tile switch
				{
					TileWithPiece twp when twp.Piece.PlayerId == game.PlayerIdToMove => game.FindValidMoves(twp),
					_ => Array.Empty<Position>()
				};
				chessBoardView.MarkTilesWithValidMoves(validMoves, doMark: true);
			}
			else
			{
				tileViewLastClicked.MarkAsSelected(doMark: false);
				chessBoardView.MarkTilesWithValidMoves(validMoves, doMark: false);
			}
			tileViewLastClicked = tileView;
		}
	}
}