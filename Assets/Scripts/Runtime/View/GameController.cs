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
			chessBoardView.Create(game.Create(), OnTileClicked);
		}

		private void OnTileClicked(TileView tileView)
		{
			Debug.Log(tileView.Tile);
			var tile = tileView.Tile;
			
			deSelectFunc?.Invoke();
			
			if (playerAction == PlayerAction.MovePiece && validMoves.Any(pos => pos == tile.Position))
			{
					var (beforeMoveTile, afterMoveTile, opponentInCheckList, hasGameEnded) = game.MovePiece(selectedTilePiece, tile.Position);
					chessBoardView.InjectTiles(new [] {beforeMoveTile, afterMoveTile});
					foreach (var t in opponentInCheckList)
					{
						if (t.checktype != CheckType.NoCheck)
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
			
			validMoves = game.FindValidMoves(twp);
			var validMovesArr = validMoves as Position[] ?? validMoves.ToArray();
			chessBoardView.MarkTiles(validMovesArr, TileMarkType.ValidMove);
			deSelectFunc = () =>
			{
				tileView.MarkTile(TileMarkType.Normal);
				chessBoardView.MarkTiles(validMovesArr, TileMarkType.Normal);
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