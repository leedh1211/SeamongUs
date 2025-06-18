using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.Lobby
{
    public class MakeRoomPopupUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject makeRoomPopup;
        [SerializeField] private TMP_InputField roomNameText;
        [SerializeField] private TMP_InputField maxPlayersText;
        [SerializeField] private Toggle secretRoomToggle;

        private bool isFirstOpenPopup = true;


        public void OnMakeRoomPopup()
        {
            makeRoomPopup.SetActive(true);
            if (isFirstOpenPopup)
            {
                roomNameText.caretWidth = 0;
                maxPlayersText.caretWidth = 0;
                isFirstOpenPopup = false;
                maxPlayersText.contentType = TMP_InputField.ContentType.IntegerNumber;
                maxPlayersText.onValueChanged.AddListener(OnInputChanged);
                maxPlayersText.ForceLabelUpdate();
            }
        }

        public void OnCloseMakeRoomPopup()
        {
            roomNameText.text = "";
            maxPlayersText.text = "";
            secretRoomToggle.isOn = false;
            makeRoomPopup.SetActive(false);
        }

        public void makeRoomBtnClicked()
        {
            string roomName = roomNameText.text;
            int MaxPlayers = int.Parse(maxPlayersText.text);
            bool IsVisible = secretRoomToggle.isOn;
            NetworkManager.Instance.MakeRoom(roomName, MaxPlayers, IsVisible);
            OnCloseMakeRoomPopup();
        }

        private void OnInputChanged(string input)
        {
            if (int.TryParse(input, out int number))
            {
                int minValue = 1;
                int maxValue = 10;
                if (number < minValue)
                    maxPlayersText.text = minValue.ToString();
                else if (number > maxValue)
                    maxPlayersText.text = maxValue.ToString();
            }
            else if (!string.IsNullOrEmpty(input)) // 입력이 숫자가 아닐 경우
            {
                maxPlayersText.text = "";
            }
        }
    }
}