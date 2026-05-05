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
		[SerializeField] private GameOverView gameOverView;
		[SerializeField] private string[] playerLabels = { "White", "Black" };

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
			gameOverView.Create();
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
					var winner = playersEndResult.FirstOrDefault(kvp => kvp.Value == Result.Win);
					var outcome = winner.Key != 0 ? $"{playerLabels[winner.Key - 1]} won!" : "Draw!";
					gameOverView.Show($"Game Over:\n{outcome}");
					return;
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
