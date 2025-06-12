using System.Collections;
using System.Text;
using _02_Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : Singleton<PlayerManager>
{
    public PlayerData playerData;
    public void Init(PlayerData data)
    {
        playerData = data;
        Debug.Log($"로그인된 유저: {data.name}");
    }
}