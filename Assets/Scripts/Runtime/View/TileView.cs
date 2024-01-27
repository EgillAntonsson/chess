using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.View
{
	public class TileView : MonoBehaviour
	{
		public Tile Tile { get; private set; }
		private Action<TileView> onTileClicked;
		private readonly Dictionary<TileMarkType, Color> colorByTileMarkType = new()
		{
			{TileMarkType.Normal, new Color(0f, 0f, 1f, 0.1f)},
			{TileMarkType.Selected, new Color(0f, 1f, 0f, 0.5f)},
			{TileMarkType.ValidMove, new Color(1f, 1f, 0f, 0.5f)},
			{TileMarkType.Check, new Color(1f, 0f, 0f, 0.5f)}
		};
		private GameObject piece;

		public void Create(Tile tile, PiecePrefabMapper mapper, Action<TileView> tileClicked)
		{
			InjectTile(tile, mapper);
			Draw(tile, mapper);
			onTileClicked = tileClicked;
		}

		public void InjectTile(Tile tile, PiecePrefabMapper mapper)
		{
			Tile = tile;
			Draw(tile, mapper);
		}

		private void Draw(Tile tile, PiecePrefabMapper mapper)
		{
			if (piece != null)
			{
				Destroy(piece);
			}
			var (success, prefab) = mapper.TryGetPiecePrefab(tile);
			if (success)
			{
				piece = Instantiate(prefab, transform);
			}
			
			MarkTile(TileMarkType.Normal);
		}

		private void OnMouseDown()
		{
			onTileClicked?.Invoke(this);
		}
		
		public void MarkTile(TileMarkType tileMarkType)
		{
			GetComponent<Renderer>().material.color = colorByTileMarkType[tileMarkType];
		}
	}
}