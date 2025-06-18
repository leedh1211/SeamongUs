using UnityEngine;

namespace _02_Scripts.UI
{
    public class MapUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject MapUIPanel;
        private bool IsOpenMapUIPopup;

        public void OnOffMapUI()
        {
            if (IsOpenMapUIPopup)
            {
                HideMapUIPopup();
            }
            else
            {
                OpenMapUIPopup();
            }
        }

        public void OpenMapUIPopup()
        {
            IsOpenMapUIPopup = true;
            MapUIPanel.SetActive(true);
        }
        
        public void HideMapUIPopup()
        {
            IsOpenMapUIPopup = false;
            MapUIPanel.SetActive(false);
        }
    }
}