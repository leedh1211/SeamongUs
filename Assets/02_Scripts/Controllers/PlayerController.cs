using System.Collections;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun , IPunObservable
{
    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 5f;
    private float currentMoveSpeed;
    private Vector2 moveInput;
    private Rigidbody2D rb;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float jumpDuration = 0.4f;
    [SerializeField] private PlayerInfo _playerInfo;

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
    [SerializeField] private float lerpSpeed = 10f; // 위치 보간 속도

    [Header("Ghost Settings")]
    [SerializeField] private float ghostMoveSpeed = 3f;         // 유령 상태 이동 속도
    [SerializeField] private float ghostAlpha = 0.5f;       // 스프라이트 투명도
    private bool isDead = false;
    private bool isGhost = false;

    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int SpeedHash = Animator.StringToHash("currentMoveSpeed");

    private Vector3 shadowOriginalScale;
    private Animator animator;
    private Vector3 networkPosition;
    [SerializeField] private Transform playerSprite;
    private static readonly int IsJumpingHash = Animator.StringToHash("isJumping");
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
        animator = GetComponentInChildren<Animator>();
        Debug.Log("Animator loaded from: " + animator.gameObject.name);
        
        _playerInfo.currentPlayer = photonView.Owner;
    }
    
    private void Start()
    {
        if (photonView.IsMine)
            StartCoroutine(WaitAndRegister());
    }
    
    
    private IEnumerator WaitAndRegister()
    {
        yield return new WaitUntil(() => MissionManager.Instance != null);
        MissionManager.Instance.RegisterLocalPlayer(this);
    }
    
    private void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            // 1. 일정 거리 이상일 경우 바로 순간이동 (패킷 손실 or 위치 차이 큼)
            if (Vector3.Distance(transform.position, networkPosition) > 2f)
            {
                transform.position = networkPosition;
            }
            else
            {
                // 2. 부드럽게 보간 이동
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * lerpSpeed);
            }
        }
        else
        {
            HandleMovement();
        }
    }

    // 플레이어 이동 처리 ─ 고스트/일반 구분
    private void HandleMovement()
    {
        if (isGhost)          
        {
            // 고스트 전용 이동 (Transform 직접 이동)
            Vector2 movement = moveInput * ghostMoveSpeed;
            transform.position += (Vector3)movement * Time.fixedDeltaTime;

            
            return;
        }

        // 2) 일반 상태
        Vector2 movementNormal = moveInput * currentMoveSpeed;
        rb.velocity = movementNormal;

        animator.SetFloat("currentMoveSpeed", rb.velocity.magnitude);

        // 이동 방향에 따른 FlipX (점프 중엔 유지)
        if (!jumping && moveInput.x != 0 && playerSprite != null)
        {
            var sr = playerSprite.GetComponent<SpriteRenderer>();
            if (sr != null) sr.flipX = moveInput.x < 0;
        }
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
        animator.SetBool(IsJumpingHash, true);
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
        jumping = false;

        animator.SetBool(IsJumpingHash, false);

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

    
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
        }
    }

    public void Die()
    {
        if (isDead) return;               //  두 번 호출 금지
        isDead = true;

        Debug.Log("[StatManager] 플레이어 사망 처리 호출");

        if (animator != null)
            animator.SetTrigger(DieHash); //  사망 애니메이션 재생

        StartCoroutine(DeathSequence());  //  1초 뒤 유령 전환
    }

    private IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(1f);  // ← 'Player_Dying' 길이에 맞춰 조정

        //  유령(Ghost) 상태로 전환
        gameObject.layer = LayerMask.NameToLayer("Ghost");

        // 스프라이트 반투명
        if (playerSprite != null)
        {
            SpriteRenderer sr = playerSprite.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = ghostAlpha;
                sr.color = c;
            }
        }

        // Rigidbody 비활성 (Physics 충돌 X)
        if (rb != null)
            rb.simulated = false;   // 또는 rb.isKinematic = true;

        isGhost = true;
    }


}
