using System.Collections;
using System.Text;
using _02_Scripts.Login.Player;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTestManager : Singleton<PlayerTestManager>
{
    public PlayerTestData playerData;
    public void Init(PlayerTestData data)
    {
        playerData = data;
        Debug.Log($"로그인된 유저: {data.name}");
    }
}