using System;
using UnityEngine;

namespace Chess.View
{
	public class TileView : MonoBehaviour
	{
		public Tile Tile { get; private set; }
		private Action<Tile> onTileClicked;

		public void Create(Tile tile, PiecePrefabMapper mapper, Action<Tile> tileClicked)
		{
			Tile = tile;
			onTileClicked = tileClicked;

			var (success, prefab) = mapper.TryGetPiecePrefab(tile);
			if (success)
			{
				var piece = Instantiate(prefab, transform);
			}
			
			var rend = GetComponent<Renderer>();
			var color = UnityEngine.Color.blue;
			color.a = 0.1f;
			rend.material.color = color;
		}

		private void OnMouseDown()
		{
			onTileClicked?.Invoke(Tile);
		}
	}
}