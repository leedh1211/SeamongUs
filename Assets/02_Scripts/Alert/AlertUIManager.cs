using System;
using TMPro;
using UnityEngine;

namespace _02_Scripts.Alert
{
    public class AlertUIManager : Singleton<AlertUIManager>
    {
        [SerializeField]
        private GameObject alertPanel;
        [SerializeField]
        private TMP_Text alertText;


        public void OnAlert(String text)
        {
            alertPanel.SetActive(true);
            alertText.text = text;
        }

        public void OnCloseAlert()
        {
            alertText.text = "";
            alertPanel.SetActive(false);
        }
    }
}