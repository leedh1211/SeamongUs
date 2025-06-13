using _02_Scripts.Login;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.Lobby
{
    public class PlayerInfoUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject playerInfoPanel;
        [SerializeField] private Image playerImage;
        [SerializeField] private TMP_Text playerName;
        [SerializeField] private TMP_Text playerLevel;
        [SerializeField] private TMP_Text playerGold;
        
        private void OnEnable()
        {
            EventBus.Subscribe<OnPlayerInfoChanged>(HandlePlayerInfoChanged);
        }

        private void OnDisable()
        {
            EventBus.UnSubscribe<OnPlayerInfoChanged>(HandlePlayerInfoChanged);
        }

        private void HandlePlayerInfoChanged(OnPlayerInfoChanged e)
        {
            playerName.text = LoginSession.loginPlayerInfo.name;
            playerLevel.text = "Lv "+LoginSession.loginPlayerInfo.level;
            playerGold.text = LoginSession.loginPlayerInfo.gold+" G";
        }
    }
}