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
		private Position[] validMoves;
		private TileWithPiece selectedTilePiece;
		private PlayerAction playerAction = PlayerAction.SelectPiece;

		private void Start()
		{
			game = new Game(new Rules());
			chessBoardView.Create(game.Create(), OnTileClicked);
		}

		private void OnTileClicked(TileView tileView)
		{
			Debug.Log(tileView.Tile);
			var tile = tileView.Tile;
			
			deSelectFunc?.Invoke();
			
			if (playerAction == PlayerAction.MovePiece && validMoves.Any(pos => pos == tile.Position))
			{
					var (beforeMoveTiles, afterMoveTiles, opponentInCheckList, hasGameEnded) = game.MovePiece(selectedTilePiece, tile.Position);
					chessBoardView.InjectTiles(beforeMoveTiles.Concat(afterMoveTiles));
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
			
			var vm = game.FindValidMoves(twp);
			validMoves = vm as Position[] ?? vm.ToArray();
			chessBoardView.MarkTiles(validMoves, TileMarkType.ValidMove);
			deSelectFunc = () =>
			{
				tileView.MarkTile(TileMarkType.Normal);
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