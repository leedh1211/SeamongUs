using TMPro;
using UnityEngine;

public class LayerController : MonoBehaviour
{
    [Header("카메라 참조")]
    public Camera mainCamera;
    public Camera spectatorCamera;

    [Header("플레이어 시각 요소")]
    public Renderer[] renderers;
    public TMP_Text nameText;
    private void Awake()
    {
        // 런타임에 씬에서 카메라 자동 연결
        if (mainCamera == null)
            mainCamera = GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();

        if (spectatorCamera == null)
            spectatorCamera = GameObject.Find("Spectator Camera")?.GetComponent<Camera>();
    }

    public void SwitchToGhostView()
    {
        foreach (var r in renderers)
        {
            r.gameObject.layer = LayerMask.NameToLayer("Ghost");
        }

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(false);

        if (spectatorCamera != null)
            spectatorCamera.gameObject.SetActive(true);
        if (nameText != null)
            nameText.enabled = false;
    }
}