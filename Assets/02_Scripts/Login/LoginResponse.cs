using _02_Scripts.Login.Player;

public class LoginResponse
{
    public string message { get; set; }
    public bool isLogin { get; set; }
    public PlayerResponseData data { get; set; }
}