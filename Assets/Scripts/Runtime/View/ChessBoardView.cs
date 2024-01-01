using System;
using UnityEngine;

namespace Chess.View
{
	public class ChessBoardView : MonoBehaviour
	{
		[SerializeField] private GameObject tile;
		[SerializeField] private PiecePrefabMapper piecePrefabMapper;
		public void Create(Board board, Action<Tile> onTileClicked)
		{
			var tiles = board.Tiles;
			var boardSize = board.Size;
			for (var row = 0; row < boardSize; row++)
			{
				for (var col = 0; col < boardSize; col++)
				{
					var t = Instantiate(tile, new Vector3(col, 0, row), Quaternion.identity, transform);
					var tileView = t.GetComponent<TileView>();
					tileView.Create(tiles[col, row], piecePrefabMapper, onTileClicked);
				}
			}
			
		}
	}
}