using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

namespace Chess.View
{
    public class PromotionSelectionView : MonoBehaviour
    {
        [SerializeField] private List<ButtonView> buttons;

        private static PieceType selectedPromotionType;
        private static bool hasSelectedPromotionType;

        public void Create(PieceType[] promotionChoices)
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                var btn = buttons[i];
                var pieceType = promotionChoices[i];

                var textComponent = btn.GetComponentInChildren<Text>();
                textComponent.text = pieceType.ToString();
                btn.pieceType = pieceType;
                
                btn.onClick.AddListener(() =>
                {
                    selectedPromotionType = btn.pieceType;
                    hasSelectedPromotionType = true;
                });
            }

            gameObject.SetActive(false);
        }
        
        public async Task<PieceType> PromoteAsync()
        {
            gameObject.SetActive(true);
            while (!hasSelectedPromotionType)
            {
                await Task.Yield();
            }
            hasSelectedPromotionType = false; // resetting for the next time.
            gameObject.SetActive(false);
            return selectedPromotionType;
        }
    }
}
