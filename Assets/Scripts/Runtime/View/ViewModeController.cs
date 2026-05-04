using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.View
{
	[DefaultExecutionOrder(100)]
	public class ViewModeController : MonoBehaviour
	{
		[SerializeField] private ChessBoardView chessBoardView;
		[SerializeField] private Camera mainCamera;
		[SerializeField] private PiecePrefabMapper mapper3D;
		[SerializeField] private PiecePrefabMapper mapper2D;
		[SerializeField] private CameraPose pose3D;
		[SerializeField] private CameraPose pose2D;
		[SerializeField] private Button toggleButton;
		[SerializeField] private TextMeshProUGUI toggleLabel;
		[SerializeField] private ViewMode defaultMode = ViewMode.ThreeD;

		private ViewMode currentMode;

		private void Start()
		{
			toggleButton.onClick.AddListener(ToggleView);
			ApplyMode(defaultMode);
		}

		private void ToggleView()
		{
			ApplyMode(currentMode == ViewMode.ThreeD ? ViewMode.TwoD : ViewMode.ThreeD);
		}

		private void ApplyMode(ViewMode mode)
		{
			currentMode = mode;
			var pose = mode == ViewMode.ThreeD ? pose3D : pose2D;
			var mapper = mode == ViewMode.ThreeD ? mapper3D : mapper2D;
			pose.ApplyTo(mainCamera);
			chessBoardView.Reskin(mapper);
			toggleLabel.text = mode == ViewMode.ThreeD ? "2D" : "3D";
		}
	}

	public enum ViewMode
	{
		ThreeD = 0,
		TwoD = 1
	}

	[Serializable]
	public struct CameraPose
	{
		public Vector3 position;
		public Vector3 eulerAngles;
		public bool orthographic;
		public float orthographicSize;
		public float fieldOfView;

		public void ApplyTo(Camera camera)
		{
			camera.transform.SetPositionAndRotation(position, Quaternion.Euler(eulerAngles));
			camera.orthographic = orthographic;
			camera.orthographicSize = orthographicSize;
			camera.fieldOfView = fieldOfView;
		}
	}
}
