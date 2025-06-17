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
        // Main Camera는 태그로 찾기
        if (mainCamera == null)
            mainCamera = GameObject.FindWithTag("MainCamera")?.GetComponent<Camera>();

        // Spectator Camera도 태그로 찾기 (비활성 상태도 OK)
        if (spectatorCamera == null)
        {
            GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            foreach (var go in allObjects)
            {
                if (go.CompareTag("SpectatorCamera"))
                {
                    spectatorCamera = go.GetComponent<Camera>();
                    break;
                }
            }
        }
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