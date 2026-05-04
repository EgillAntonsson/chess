using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess.View
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private ChessBoardView chessBoardView;
		[SerializeField] private PromotionSelectionView promotionSelectionView;

		private Game game;
		private Action deSelectFunc;
		private Position[] validMoves;
		private TileWithPiece selectedTilePiece;
		private PlayerAction playerAction = PlayerAction.SelectPiece;
		private List<(Position position, TileMarkType mark)> persistentMarks = new();

		private void Start()
		{
			var rules = new Rules();
			var chessboard = new ChessBoard(rules);
			game = new Game(rules, chessboard);
			chessBoardView.Create(game.Create(), OnTileClicked);
			promotionSelectionView.Create(rules.PromotionChoices);
		}

		private async void OnTileClicked(TileView tileView)
		{
			if (game.GameHasEnded || game.PromotionIsOccuring)
			{
				return;
			}
			var tile = tileView.Tile;
			deSelectFunc?.Invoke();

			if (playerAction == PlayerAction.MovePiece && validMoves.Any(pos => pos == tile.Position))
			{
				var (changedTiles, playersWithCheckTile, playersEndResult) = await game.MovePiece(selectedTilePiece, tile.Position, promotionSelectionView.PromoteAsync);
				chessBoardView.InjectTiles(changedTiles);

				persistentMarks.Clear();
				foreach (var (player, checkTilePos) in playersWithCheckTile)
				{
					var mark = TileMarkTypeUtil.ConvertFromCheckType(player.IsInCheckType);
					chessBoardView.MarkTile(checkTilePos, mark);
					if (mark != TileMarkType.Normal)
					{
						persistentMarks.Add((checkTilePos, mark));
					}
				}

				if (game.GameHasEnded)
				{
					Debug.Log("GAME HAS ENDED:");
					foreach (var kvp in playersEndResult.Where(kvp => kvp.Value == Result.Win))
					{
						Debug.Log($"'{kvp.Key}' WON.");
						return;
					}
					foreach (var kvp in playersEndResult.Where(kvp => kvp.Value == Result.Draw))
					{
						Debug.Log("IT'S A DRAW.");
					}
				}
				playerAction = PlayerAction.SelectPiece;
				return;
			}

			if (tile is not TileWithPiece twp || twp.Piece.PlayerId != game.PlayerIdToMove)
			{
				playerAction = PlayerAction.SelectPiece;
				return;
			}

			selectedTilePiece = twp;
			var vm = game.FindMovePositions(twp);
			validMoves = vm as Position[] ?? vm.ToArray();
			chessBoardView.MarkTile(twp.Position, TileMarkType.Selected);
			chessBoardView.MarkTiles(validMoves, TileMarkType.ValidMove);
			deSelectFunc = () =>
			{
				chessBoardView.MarkTile(twp.Position, TileMarkType.Normal);
				chessBoardView.MarkTiles(validMoves, TileMarkType.Normal);
				foreach (var (pos, mark) in persistentMarks)
				{
					chessBoardView.MarkTile(pos, mark);
				}
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
