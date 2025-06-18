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
        [SerializeField] private SpriteRenderer bodyRenderer;
        [SerializeField] private Animator animator;
        
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
            ChangeInfo();
        }

        public void ChangeInfo()
        {
            playerName.text = LoginSession.loginPlayerInfo.name;
            playerLevel.text = "Lv "+LoginSession.loginPlayerInfo.level;
            playerGold.text = LoginSession.loginPlayerInfo.gold+" G";
            bodyRenderer.sprite = AvatarManager.Instance.GetSprite(LoginSession.loginPlayerInfo.current_character);
            animator.runtimeAnimatorController = AvatarManager.Instance.GetAnim(LoginSession.loginPlayerInfo.current_character);
        }

        public void Start()
        {
            ChangeInfo();
        }
    }
}