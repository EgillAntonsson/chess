using System;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.View
{
	public class TileView : MonoBehaviour
	{
		[SerializeField] private Renderer highlightRenderer;

		public Tile Tile { get; private set; }
		private Action<TileView> onTileClicked;
		private readonly Dictionary<TileMarkType, Color> colorByTileMarkType = new()
		{
			{TileMarkType.Normal, new Color(0f, 0f, 0f, 0f)},
			{TileMarkType.Selected, new Color(0f, 1f, 0f, 0.5f)},
			{TileMarkType.ValidMove, new Color(0f, 0.5f, 1f, 0.5f)},
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

		public void Reskin(PiecePrefabMapper mapper)
		{
			DrawPiece(Tile, mapper);
		}

		private void Draw(Tile tile, PiecePrefabMapper mapper)
		{
			DrawPiece(tile, mapper);
			MarkTile(TileMarkType.Normal);
		}

		private void DrawPiece(Tile tile, PiecePrefabMapper mapper)
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
		}

		private void OnMouseDown()
		{
			onTileClicked?.Invoke(this);
		}
		
		public void MarkTile(TileMarkType tileMarkType)
		{
			highlightRenderer.material.color = colorByTileMarkType[tileMarkType];
		}
	}
}