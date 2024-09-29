using System;
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
				
				foreach (var t in playersWithCheckTile)
				{
					chessBoardView.MarkTile(t.checkTilePos, TileMarkTypeUtil.ConvertFromCheckType(t.player.IsInCheckType));
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
				return;
			}
			
			selectedTilePiece = twp;
			
			var vm = game.FindMovePositions(twp);
			validMoves = vm as Position[] ?? vm.ToArray();
			chessBoardView.MarkTiles(validMoves, TileMarkType.ValidMove);
			deSelectFunc = () =>
			{
				chessBoardView.MarkTiles(validMoves, TileMarkType.Normal);
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