using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerSlot
{
    public GameObject slotObject;
    public Text nicknameText;
    public Image characterIcon;

    public void SetPlayer(PlayerInfo info)
    {
        // Check if the slot is already active
        slotObject.SetActive(true);
        nicknameText.text = info.Nickname;
        characterIcon.sprite = Resources.Load<Sprite>($"Characters/{info.SpriteData}");
    }

    public void ClearSlot()
    {
        // Clear the slot information
        slotObject.SetActive(false);
    }
}
