using System;
using UnityEngine;

namespace Chess.View
{
	public class TileView : MonoBehaviour
	{
		public Tile Tile { get; private set; }
		private Action<TileView> onTileClicked;
		private readonly Color defaultColor = new Color(0f, 0f, 1f, 0.1f);
		private readonly Color selectedColor = new Color(0f, 1f, 0f, 0.5f);
		private readonly Color validMoveColor = new Color(1f, 1f, 0f, 0.5f);
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
			
			GetComponent<Renderer>().material.color = defaultColor;	
		}

		private void OnMouseDown()
		{
			onTileClicked?.Invoke(this);
		}
		
		public void MarkAsSelected(bool doMark)
		{
			GetComponent<Renderer>().material.color = doMark ? selectedColor : defaultColor;
		}
		
		public void MarkWithValidMove(bool doMark)
		{
			GetComponent<Renderer>().material.color = doMark ? validMoveColor : defaultColor;
		}
	}
}