using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("Jump")]
    public float jumpHeight = 1f;
    public float jumpDuration = 0.4f;
    private bool isJumping;
    private Vector3 originalPos;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalPos = transform.position;
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

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && !isJumping)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, interactRange, jumpableLayer);
            if (hit.collider != null)
            {
                StartCoroutine(JumpOverObstacle());
            }
        }
    }

    private System.Collections.IEnumerator JumpOverObstacle()
    {
        isJumping = true;
        Vector3 peak = originalPos + new Vector3(0, jumpHeight, 0);
        float t = 0f;

        while (t < jumpDuration / 2)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(originalPos, peak, t / (jumpDuration / 2));
            yield return null;
        }

        t = 0f;
        while (t < jumpDuration / 2)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(peak, originalPos, t / (jumpDuration / 2));
            yield return null;
        }

        isJumping = false;
    }

    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("인벤토리 열기");
            OnOpenInventory?.Invoke();
        }
    }
}
