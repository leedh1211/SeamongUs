using UnityEngine;

public class IntroImageScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;         // 이동 속도
    public float endY = 262f;              // 최종 Y 좌표

    private void Update()
    {
        if (transform.position.y < endY)
        {
            transform.position += new Vector3(0, scrollSpeed * Time.deltaTime, 0);
        }
    }
}
