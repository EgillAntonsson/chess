using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Chess.View
{
	public class GameOverView : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI messageLabel;
		[SerializeField] private Button restartButton;

		public void Create()
		{
			restartButton.onClick.AddListener(Restart);
			gameObject.SetActive(false);
		}

		public void Show(string message)
		{
			messageLabel.text = message;
			gameObject.SetActive(true);
		}

		private static void Restart()
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}
	}
}
