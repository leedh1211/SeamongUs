using UnityEngine;

public class Player : MonoBehaviour
{
    public string PlayerID { get; private set; }
    public bool IsDead { get; private set; } = false;

    public void Initialize(string id)
    {
        PlayerID = id;
    }
}
