using System;
using UnityEngine;

namespace Chess.View
{
	public class TileView : MonoBehaviour
	{
		public Tile Tile { get; private set; }
		private Action<TileView> onTileClicked;
		private Color defaultColor;

		public void Create(Tile tile, PiecePrefabMapper mapper, Action<TileView> tileClicked)
		{
			Tile = tile;
			onTileClicked = tileClicked;

			var (success, prefab) = mapper.TryGetPiecePrefab(tile);
			if (success)
			{
				var piece = Instantiate(prefab, transform);
			}
			
			var rend = GetComponent<Renderer>();
			defaultColor = UnityEngine.Color.blue;
			defaultColor.a = 0.1f;
			rend.material.color = defaultColor;
		}

		private void OnMouseDown()
		{
			onTileClicked?.Invoke(this);
		}
		
		public void MarkAsSelected(bool doMark)
		{
			var markColor = Color.green;
			markColor.a = 0.5f;
			var rend = GetComponent<Renderer>();
			rend.material.color = doMark ? markColor : defaultColor;
		}
		
		public void MarkWithValidMove(bool doMark)
		{
			var markColor = Color.yellow;
			markColor.a = 0.5f;
			var rend = GetComponent<Renderer>();
			rend.material.color = doMark ? markColor : defaultColor;
		}
	}
}