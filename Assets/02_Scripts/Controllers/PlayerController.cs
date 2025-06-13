using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 5f;
    private float currentMoveSpeed;
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
    [SerializeField] private Transform visual;      // 캐릭터 스프라이트
    [SerializeField] private Transform shadow;      // 그림자 (선택)
    private Vector3 shadowOriginalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();                 // ← 다시 넣기
        if (rb == null)
            Debug.LogError("Rigidbody2D 컴포넌트가 없습니다!", this);

        if (visual == null)
            Debug.LogError("visual 트랜스폼이 할당되지 않았습니다!", this);
        if (shadow != null)
            shadowOriginalScale = shadow.localScale;
        visualDefaultPos = visual.localPosition;
        currentMoveSpeed = baseMoveSpeed;
    }


    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 movement = moveInput * currentMoveSpeed;
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
                Debug.Log("미션 상호작용 시도");
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
                Debug.Log("킬 시도");
                OnKill?.Invoke();
            }
        }
    }

    public void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            Debug.Log("점프 시도됨");
            StartCoroutine(JumpCoroutine());
        }
    }


    private bool IsJumpableAhead()
    {
        // 탑뷰니까 “앞”은 moveInput 방향.
        if (moveInput == Vector2.zero) return false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
                                             moveInput.normalized,
                                             0.6f,    // 거리
                                             jumpableLayer);
        return hit.collider != null;
    }

    private System.Collections.IEnumerator JumpCoroutine()
    {
        jumping = true;
        float half = jumpDuration * 0.5f;
        float t = 0;

        // 상승
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

        // 하강
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
            Debug.Log("인벤토리 열기");
            OnOpenInventory?.Invoke();
        }
    }

    public void ModifySpeed(float amount)
    {
        currentMoveSpeed += amount;
        currentMoveSpeed = Mathf.Max(0f, currentMoveSpeed); // 음수 방지

        Debug.Log($"[Speed] 이동속도 변경됨: {currentMoveSpeed}");
    }

    // 선택적으로 초기화용 메서드
    public void ResetSpeed()
    {
        currentMoveSpeed = baseMoveSpeed;
    }
}
