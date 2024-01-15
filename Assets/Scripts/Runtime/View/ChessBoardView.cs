using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess.View
{
	public class ChessBoardView : MonoBehaviour
	{
		[SerializeField] private GameObject tile;
		[SerializeField] private PiecePrefabMapper piecePrefabMapper;

		private Dictionary<Position, TileView> tileViewByPosition;
		public void Create(Board board, Action<TileView> onTileClicked)
		{
			var tiles = board.Tiles;
			var boardSize = board.Size;
			tileViewByPosition = new Dictionary<Position, TileView>(boardSize * boardSize);
			for (var row = 0; row < boardSize; row++)
			{
				for (var col = 0; col < boardSize; col++)
				{
					var t = Instantiate(tile, new Vector3(col, 0, row), Quaternion.identity, transform);
					var tileView = t.GetComponent<TileView>();
					tileView.Create(tiles[col, row], piecePrefabMapper, onTileClicked);
					tileViewByPosition.Add(new Position(col, row), tileView);
				}
			}
			
		}
		
		public void InjectTiles(IEnumerable<Tile> tiles)
		{
			foreach (var t in tiles)
			{
				tileViewByPosition[t.Position].InjectTile(t, piecePrefabMapper);
			}
		}
		
		public void MarkTilesWithValidMoves(IEnumerable<Position> validMoves, bool doMark)
		{
			foreach (var position in validMoves)
			{
				tileViewByPosition[position].MarkWithValidMove(doMark: doMark);
			}
		}
	}
}