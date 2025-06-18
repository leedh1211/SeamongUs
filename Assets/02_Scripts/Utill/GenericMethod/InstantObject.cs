using UnityEngine;

public class InstantObject : MonoBehaviour
{
    //활성화 시 특정 시간 이후, 자동으로 비활성화 되는 오브젝트

    [SerializeField] private float duration;
    private void OnEnable()
    {
        Invoke(nameof(Disable), duration);
    }

    private void Disable()
    {
        this.gameObject.SetActive(false);
    }
}
