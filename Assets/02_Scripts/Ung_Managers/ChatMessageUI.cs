using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace _02_Scripts.Ung_Managers
{

    public class ChatMessageUI : MonoBehaviour
    {
        public TMP_Text nicknameText;
        public TMP_Text messageText;
        public Image avatarImage;
        public HorizontalLayoutGroup layoutGroup;

        public void SetMessage(string nickname, string message, Sprite avatar, bool isMine)
        {
            nicknameText.text = nickname;
            messageText.text = message;
            avatarImage.sprite = avatar;

            if (isMine)
            {
                // 오른쪽 정렬
                layoutGroup.childAlignment = TextAnchor.MiddleRight;
                avatarImage.transform.SetAsLastSibling(); // 아바타 오른쪽
                nicknameText.alignment = TextAlignmentOptions.MidlineRight;
                messageText.alignment = TextAlignmentOptions.MidlineRight;
            }
            else
            {
                // 왼쪽 정렬
                layoutGroup.childAlignment = TextAnchor.MiddleLeft;
                avatarImage.transform.SetAsFirstSibling(); // 아바타 왼쪽
                nicknameText.alignment = TextAlignmentOptions.MidlineLeft;
                messageText.alignment = TextAlignmentOptions.MidlineLeft;
            }
        }
    }
}