using System.Collections;
using UnityEngine;
using Cinemachine;

public class FollowVirtualCamera : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        StartCoroutine(AssignFollowTarget());
    }

    IEnumerator AssignFollowTarget()
    {
        PlayerController local = null;

        // PlayerController 컴포넌트 중 photonView.IsMine이 true인 것을 찾을 때까지 대기
        yield return new WaitUntil(() =>
        {
            foreach (var playerComp in FindObjectsOfType<PlayerController>())
            {
                if (playerComp.photonView.IsMine)
                {
                    local = playerComp;
                    return true; // 조건 만족
                }
            }
            return false; // 조건 불만족, 계속 대기
        });

        virtualCamera.Follow = local.transform; // 찾은 PlayerController의 transform을 Follow로 설정
    }
}
