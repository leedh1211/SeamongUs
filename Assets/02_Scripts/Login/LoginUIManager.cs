using UnityEngine;

namespace _02_Scripts.Login
{
    public class LoginUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject LoginPopup;

        public void ShowLoginPopup()
        {
            LoginPopup.SetActive(true);
        }
        
        public void HideLoginPopup()
        {
            LoginPopup.SetActive(false);
        }
    }
}