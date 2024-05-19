using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Chess.View
{
	public class GameController : MonoBehaviour
	{
		[SerializeField] private ChessBoardView chessBoardView;

		private Game game;
		private Action deSelectFunc;
		private Position[] validMoves;
		private TileWithPiece selectedTilePiece;
		private PlayerAction playerAction = PlayerAction.SelectPiece;

		private void Start()
		{
			game = new Game(new Rules());
			chessBoardView.Create(game.Create(), OnTileClicked);
		}

		private async void OnTileClicked(TileView tileView)
		{
			Debug.Log(tileView.Tile);
			var tile = tileView.Tile;
			
			deSelectFunc?.Invoke();

			if (playerAction == PlayerAction.MovePiece && validMoves.Any(pos => pos == tile.Position))
			{
				var (changedTiles, opponentInCheckList, hasGameEnded) = await game.MovePiece(selectedTilePiece, tile.Position, PromoteAsync);
				chessBoardView.InjectTiles(changedTiles);
				foreach (var t in opponentInCheckList)
				{
					if (t.Item1.IsInCheckType != CheckType.NoCheck)
					{
						chessBoardView.MarkTile(t.checkTile.Position, TileMarkType.Check);
					}
				}

				if (hasGameEnded)
				{
					Debug.Log("Game has ended");
				}

				playerAction = PlayerAction.SelectPiece;
				return;
			}

			if (tile is not TileWithPiece twp || twp.Piece.PlayerId != game.PlayerIdToMove)
			{
				return;
			}
			
			tileView.MarkTile(TileMarkType.Normal);
			selectedTilePiece = twp;
			
			var vm = game.FindValidMoves(twp);
			validMoves = vm as Position[] ?? vm.ToArray();
			chessBoardView.MarkTiles(validMoves, TileMarkType.ValidMove);
			deSelectFunc = () =>
			{
				tileView.MarkTile(TileMarkType.Normal);
				chessBoardView.MarkTiles(validMoves, TileMarkType.Normal);
			};
			
			playerAction = PlayerAction.MovePiece;
			return;
		}

		public static async Task<PieceType> PromoteAsync(TileWithPiece twp)
		{
			// Wait for the user to make a selection
			await Task.Delay(1000); // This will block until the user makes a selection
			return PieceType.Queen;
		}
		
	}

	public enum PlayerAction
	{
		SelectPiece = 0,
		MovePiece = 1
	}
}