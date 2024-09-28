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
		public void Create(Tile[,] boardTiles, Action<TileView> onTileClicked)
		{
			var boardSize = Board.GetSize(boardTiles);
			tileViewByPosition = new Dictionary<Position, TileView>(boardSize * boardSize);
			for (var row = 0; row < boardSize; row++)
			{
				for (var col = 0; col < boardSize; col++)
				{
					var t = Instantiate(tile, new Vector3(col, 0, row), Quaternion.identity, transform);
					var tileView = t.GetComponent<TileView>();
					tileView.Create(boardTiles[col, row], piecePrefabMapper, onTileClicked);
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
		
		public void MarkTiles(IEnumerable<Position> tilePositions, TileMarkType tileMarkType)
		{
			foreach (var position in tilePositions)
			{
				tileViewByPosition[position].MarkTile(tileMarkType);
			}
		}
		
		public void MarkTile(Position tilePosition, TileMarkType tileMarkType)
		{
			MarkTiles(new [] {tilePosition}, tileMarkType);
		}
	}
}