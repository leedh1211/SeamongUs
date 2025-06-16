using _02_Scripts.Ung_Managers;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PlayerVisual : MonoBehaviourPunCallbacks
{
    public SpriteRenderer bodyRenderer;
    public Animator animator;

    private void Start()
    {
        ApplyAvatar(); // 최초 반영
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player target, Hashtable changedProps)
    {
        if (target == photonView.Owner && changedProps.ContainsKey(PlayerPropKey.Spr))
            ApplyAvatar();
    }

    private void ApplyAvatar()
    {
        if (photonView.Owner.CustomProperties.TryGetValue(PlayerPropKey.Spr, out object idxObj))
        {
            int idx = (int)idxObj;
            bodyRenderer.sprite = AvatarManager.Instance.GetSprite(idx);
            animator.runtimeAnimatorController = AvatarManager.Instance.GetAnim(idx);
        }
    }
}