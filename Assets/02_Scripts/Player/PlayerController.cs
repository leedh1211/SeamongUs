using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.4f;

    private Vector3 visualDefaultPos;
    private bool jumping;

    [Header("Layer Masks")]
    public LayerMask interactLayer;
    public LayerMask playerLayer;
    public LayerMask jumpableLayer;

    [Header("Detection")]
    public float interactRange = 1.5f;
    public float killRange = 1.2f;

    [Header("UI Callback Hooks")]
    public System.Action OnInteract;
    public System.Action OnKill;
    public System.Action OnOpenInventory;

    [Header("Refs")]
    [SerializeField] private Transform visual;      // ĳ���� ��������Ʈ
    [SerializeField] private Transform shadow;      // �׸��� (����)
    private Vector3 shadowOriginalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();                 // �� �ٽ� �ֱ�
        if (rb == null)
            Debug.LogError("Rigidbody2D ������Ʈ�� �����ϴ�!", this);

        if (visual == null)
            Debug.LogError("visual Ʈ�������� �Ҵ���� �ʾҽ��ϴ�!", this);
        if (shadow != null)
            shadowOriginalScale = shadow.localScale;
        visualDefaultPos = visual.localPosition;
    }


    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 movement = moveInput * moveSpeed;
        rb.velocity = movement;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Collider2D target = Physics2D.OverlapCircle(transform.position, interactRange, interactLayer);
            if (target != null)
            {
                Debug.Log("�̼� ��ȣ�ۿ� �õ�");
                OnInteract?.Invoke();
            }
        }
    }

    public void OnKillInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Collider2D target = Physics2D.OverlapCircle(transform.position, killRange, playerLayer);
            if (target != null)
            {
                Debug.Log("ų �õ�");
                OnKill?.Invoke();
            }
        }
    }

    public void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("���� �õ���");
            StartCoroutine(JumpCoroutine());
        }
    }


    private bool IsJumpableAhead()
    {
        // ž��ϱ� ���ա��� moveInput ����.
        if (moveInput == Vector2.zero) return false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                                             moveInput.normalized,
                                             0.6f,    // �Ÿ�
                                             jumpableLayer);
        return hit.collider != null;
    }

    private System.Collections.IEnumerator JumpCoroutine()
    {
        jumping = true;
        float half = jumpDuration * 0.5f;
        float t = 0;

        // ���
        while (t < half)
        {
            t += Time.deltaTime;
            float h = Mathf.Lerp(0, jumpHeight, t / half);
            visual.localPosition = visualDefaultPos + Vector3.up * h;
            if (shadow)
            {
                shadow.localScale = Vector3.Lerp(shadowOriginalScale, shadowOriginalScale * 0.8f, t / half);
            }

            yield return null;
        }

        // �ϰ�
        t = 0;
        while (t < half)
        {
            t += Time.deltaTime;
            float h = Mathf.Lerp(jumpHeight, 0, t / half);
            visual.localPosition = visualDefaultPos + Vector3.up * h;
            if (shadow)
            {
                shadow.localScale = Vector3.Lerp(shadowOriginalScale, shadowOriginalScale * 0.8f, t / half);
            }

            yield return null;
        }

        visual.localPosition = visualDefaultPos;
        if (shadow) shadow.localScale = shadowOriginalScale;
        jumping = false;
    }


    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("�κ��丮 ����");
            OnOpenInventory?.Invoke();
        }
    }
}
