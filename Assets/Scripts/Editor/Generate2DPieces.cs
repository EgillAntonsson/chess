using System.Collections.Generic;
using Chess.View;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Chess.EditorTools
{
	public static class Generate2DPieces
	{
		private const string PrefabFolder = "Assets/Prefabs/2D";
		private const string MapperPath = "Assets/ScriptableObjects/PiecePrefabMapper2D.asset";

		private static readonly (PieceType type, string letter)[] Pieces =
		{
			(PieceType.Pawn, "P"),
			(PieceType.Knight, "N"),
			(PieceType.Bishop, "B"),
			(PieceType.Rook, "R"),
			(PieceType.Queen, "Q"),
			(PieceType.King, "K"),
		};

		[MenuItem("Tools/Chess/Generate 2D Piece Prefabs")]
		public static void Generate()
		{
			EnsureFolder(PrefabFolder);

			var pieceTypes = new List<PieceType>();
			var p1Prefabs = new List<GameObject>();
			var p2Prefabs = new List<GameObject>();

			foreach (var (type, letter) in Pieces)
			{
				pieceTypes.Add(type);
				p1Prefabs.Add(CreatePiecePrefab(type, letter, playerId: 1, Color.white));
				p2Prefabs.Add(CreatePiecePrefab(type, letter, playerId: 2, Color.black));
			}

			var mapper = AssetDatabase.LoadAssetAtPath<PiecePrefabMapper>(MapperPath);
			if (mapper == null)
			{
				mapper = ScriptableObject.CreateInstance<PiecePrefabMapper>();
				AssetDatabase.CreateAsset(mapper, MapperPath);
			}

			var so = new SerializedObject(mapper);
			FillEnumArray(so.FindProperty("pieceTypes"), pieceTypes);
			FillObjectArray(so.FindProperty("piecePrefabsPlayer1"), p1Prefabs);
			FillObjectArray(so.FindProperty("piecePrefabsPlayer2"), p2Prefabs);
			so.ApplyModifiedProperties();

			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

			Debug.Log($"Generated {p1Prefabs.Count + p2Prefabs.Count} 2D piece prefabs in {PrefabFolder} and mapper at {MapperPath}");
		}

		private static GameObject CreatePiecePrefab(PieceType type, string letter, int playerId, Color color)
		{
			var go = new GameObject($"{type}{playerId}");
			go.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

			var tmp = go.AddComponent<TextMeshPro>();
			tmp.text = letter;
			tmp.color = color;
			tmp.fontSize = 5f;
			tmp.alignment = TextAlignmentOptions.Center;
			tmp.fontStyle = FontStyles.Bold;
			tmp.textWrappingMode = TextWrappingModes.NoWrap;

			var path = $"{PrefabFolder}/{go.name}.prefab";
			var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
			Object.DestroyImmediate(go);
			return prefab;
		}

		private static void EnsureFolder(string path)
		{
			if (AssetDatabase.IsValidFolder(path))
			{
				return;
			}
			var parts = path.Split('/');
			var current = parts[0];
			for (var i = 1; i < parts.Length; i++)
			{
				var next = $"{current}/{parts[i]}";
				if (!AssetDatabase.IsValidFolder(next))
				{
					AssetDatabase.CreateFolder(current, parts[i]);
				}
				current = next;
			}
		}

		private static void FillEnumArray(SerializedProperty prop, List<PieceType> values)
		{
			prop.arraySize = values.Count;
			for (var i = 0; i < values.Count; i++)
			{
				prop.GetArrayElementAtIndex(i).intValue = (int)values[i];
			}
		}

		private static void FillObjectArray(SerializedProperty prop, List<GameObject> objects)
		{
			prop.arraySize = objects.Count;
			for (var i = 0; i < objects.Count; i++)
			{
				prop.GetArrayElementAtIndex(i).objectReferenceValue = objects[i];
			}
		}
	}
}
