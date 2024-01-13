using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess.View
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private ChessBoardView chessBoardView;

		private Game game;
		private Action deSelectFunc;
		private IEnumerable<Position> validMoves;
		private TileWithPiece selectedTilePiece;
		private PlayerAction playerAction = PlayerAction.SelectPiece;

		private void Start()
		{
			game = new Game(VariantFactory.Create(VariantType.Standard));
			chessBoardView.Create(game.ChessBoard, OnTileClicked);
		}

		private void OnTileClicked(TileView tileView)
		{
			Debug.Log(tileView.Tile);
			var tile = tileView.Tile;
			
			deSelectFunc?.Invoke();

			
			if (playerAction == PlayerAction.MovePiece && validMoves.Any(pos => pos == tile.Position))
			{
					game.MovePiece(selectedTilePiece, tile.Position);
					playerAction = PlayerAction.SelectPiece;
					return;
			}

			if (tile is not TileWithPiece twp || twp.Piece.PlayerId != game.PlayerIdToMove && playerAction == PlayerAction.SelectPiece)
			{
				return;
			}
			
			tileView.MarkAsSelected(doMark: true);
			selectedTilePiece = twp;
			;
			validMoves = game.FindValidMoves(twp);
			
			var validMovesArr = validMoves as Position[] ?? validMoves.ToArray();
			chessBoardView.MarkTilesWithValidMoves(validMovesArr, doMark: true);
			deSelectFunc = () =>
			{
				tileView.MarkAsSelected(doMark: false);
				chessBoardView.MarkTilesWithValidMoves(validMovesArr, doMark: false);
			};
			
			playerAction = PlayerAction.MovePiece;
			
		}
	}

	public enum PlayerAction
	{
		SelectPiece = 0,
		MovePiece = 1
	}
}