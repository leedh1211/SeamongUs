using UnityEngine;

public class GhostManager : MonoBehaviour
{
    public static GhostManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 플레이어 게임오브젝트를 받아 반투명 처리
    public void SetGhostAppearance(GameObject playerGO)
    {
        var renderers = playerGO.GetComponentsInChildren<Renderer>();

        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                Color c = mat.color;
                c.a = 0.5f;  // 반투명도 50% 예시
                mat.color = c;

                // 투명 셰이더로 바꿔야 투명 효과가 제대로 적용됩니다.
                mat.SetFloat("_Mode", 3); // Unity Standard Shader에서 Transparent 모드
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
            }
        }
    }
}
