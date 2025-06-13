using System.Collections;
using System.Text;
using _02_Scripts.Player;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTestManager : Singleton<PlayerTestManager>
{
    public PlayerDataTest playerData;
    public void Init(PlayerDataTest data)
    {
        playerData = data;
        Debug.Log($"로그인된 유저: {data.name}");
    }
}