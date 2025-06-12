using TMPro;
using UnityEngine;


namespace _02_Scripts.SignUP
{
    public class SignUpUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject SignUpPopup;
        [SerializeField] private TMP_InputField idInputField;
        [SerializeField] private TMP_InputField pwInputField;
        [SerializeField] private TMP_InputField pwcInputField;
        [SerializeField] private TMP_InputField nickNameInputField;
        [SerializeField] private TMP_InputField emailInputField;

        // public void Start()
        // {
        //     SignUpPopup.SetActive(false);
        // }

        public void ShowSignUpPopup()
        {
            SignUpPopup.SetActive(true);
        }
        
        public void HideSignUpPopup()
        {
            SignUpPopup.SetActive(false);
        }

        public void ResetSignUpPopup()
        {
            idInputField.readOnly = false;
            idInputField.text = "";
            pwInputField.text = "";
            pwcInputField.text = "";
            nickNameInputField.text = "";
            emailInputField.text = "";
        }
    }
}