using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPun, IPunObservable
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
    [SerializeField] private Transform visual;
    [SerializeField] private Transform shadow;
    [SerializeField] private float lerpSpeed = 10f;

    [Header("Ghost Settings")]
    [SerializeField] private float ghostMoveSpeed = 3f;
    [SerializeField] private float ghostAlpha = 0.5f;
    private bool isGhost = false;
       
    private bool IsUIFocused()
    {
        var sel = EventSystem.current?.currentSelectedGameObject;
        if (sel == null) return false;
        return sel.GetComponent<TMPro.TMP_InputField>() != null;
    }

    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int SpeedHash = Animator.StringToHash("currentMoveSpeed");
    private static readonly int IsJumpingHash = Animator.StringToHash("isJumping");

    private Vector3 shadowOriginalScale;
    private Animator animator;
    private Vector3 networkPosition;
    private bool facingLeft;
    private bool playDance;

    [SerializeField] private Transform playerSprite;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        visualDefaultPos = visual.localPosition;
        shadowOriginalScale = shadow != null ? shadow.localScale : Vector3.one;
        currentMoveSpeed = baseMoveSpeed;
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "GameScene" && photonView.IsMine)
        {
            StartCoroutine(WaitAndRegister());
        }
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
            if (Vector3.Distance(transform.position, networkPosition) > 2f)
                transform.position = networkPosition;
            else
                transform.position = Vector3.Lerp(transform.position, networkPosition, Time.fixedDeltaTime * lerpSpeed);
        }
        else
        {
            HandleMovement();
        }
    }

    private void HandleMovement()
    {
        if (isGhost)
        {
            transform.position += (Vector3)(moveInput * ghostMoveSpeed * Time.fixedDeltaTime);
            UpdateFacing();
            return;
        }

        rb.velocity = moveInput * currentMoveSpeed;
        animator.SetFloat(SpeedHash, rb.velocity.magnitude);
        UpdateFacing();
    }
    private void UpdateFacing()
    {
        if (moveInput.x == 0 || playerSprite == null) return;

        bool nextFacingLeft = moveInput.x < 0;
        if (nextFacingLeft != facingLeft)
        {
            facingLeft = nextFacingLeft;
            var sr = playerSprite.GetComponent<SpriteRenderer>();
            if (sr) sr.flipX = facingLeft;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsUIFocused())
            return;
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (context.performed && photonView.IsMine)
        {
            OnInteractAction();
        }
    }

    public void OnInteractAction()
    {
        Collider2D target = Physics2D.OverlapCircle(transform.position, interactRange, interactLayer);
        if (target != null) OnInteract?.Invoke();
    }

    public void OnKillInput(InputAction.CallbackContext context)
    {
        if (photonView.IsMine && context.performed)
        {
            TryKill();
        }
    }

    public void TryKill()
    {
        if (photonView.gameObject.TryGetComponent<ImposterController>(out ImposterController imposter))
        {
            imposter.TryKill();    
        }
        else
        {
            Debug.Log("This Object is Not Imposter");
        }
    }

    public void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (IsUIFocused()) return;
        if (photonView.IsMine && ctx.performed)
        {
            PhotonNetwork.RaiseEvent(
                EventCodes.PlayerJump,
                new object[] { PhotonNetwork.LocalPlayer.ActorNumber },
                new RaiseEventOptions { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable);
        }
    }

    private bool IsJumpableAhead()
    {
        if (moveInput == Vector2.zero) return false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, moveInput.normalized, 0.6f, jumpableLayer);
        return hit.collider != null;
    }

    public IEnumerator JumpCoroutine()
    {
        jumping = true;
        animator.SetBool(IsJumpingHash, true);
        float half = jumpDuration * 0.5f;
        float t = 0;

        while (t < half)
        {
            t += Time.deltaTime;
            float h = Mathf.Lerp(0, jumpHeight, t / half);
            visual.localPosition = visualDefaultPos + Vector3.up * h;
            if (shadow != null)
                shadow.localScale = Vector3.Lerp(shadowOriginalScale, shadowOriginalScale * 0.8f, t / half);
            yield return null;
        }

        jumping = false;
        animator.SetBool(IsJumpingHash, false);

        t = 0;
        while (t < half)
        {
            t += Time.deltaTime;
            float h = Mathf.Lerp(jumpHeight, 0, t / half);
            visual.localPosition = visualDefaultPos + Vector3.up * h;
            if (shadow != null)
                shadow.localScale = Vector3.Lerp(shadowOriginalScale, shadowOriginalScale * 0.8f, t / half);
            yield return null;
        }

        visual.localPosition = visualDefaultPos;
        if (shadow != null) shadow.localScale = shadowOriginalScale;
    }

    public void OnInventoryInput(InputAction.CallbackContext context)
    {
        if (context.performed && photonView.IsMine)
        {
            OnOpenInventory?.Invoke();
        }
    }

    public void ModifySpeed(float amount)
    {
        currentMoveSpeed += amount;
        currentMoveSpeed = Mathf.Max(0f, currentMoveSpeed);
    }

    public void ResetSpeed()
    {
        currentMoveSpeed = baseMoveSpeed;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(facingLeft);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            facingLeft      = (bool)  stream.ReceiveNext();
            
            var sr = playerSprite.GetComponent<SpriteRenderer>();
            if (sr) sr.flipX = facingLeft;
        }
    }

    public void OnReportInput(InputAction.CallbackContext context)
    {
        if (IsUIFocused())
            return;
        if (context.performed && photonView.IsMine)
        {
            OnReportAction();
        }
    }

    public void OnReportAction()
    {
        int deadBodyActorNum = DeadBodyManager.Instance.GetClosestDeadBodyID(transform.position);
        ReportManager.Instance.ReportBody(deadBodyActorNum);
    }
    
    private void OnVotingEnd()
    {
        foreach (var player in VoteManager.Instance.VoteResults)
        {
            // 플레이어에게 투표 결과 전송
            Debug.Log($"  {player.Key} => {player.Value}");
        }
    }

    public void Die(string Category = "killing")
    {
        if (animator != null)
            animator.SetTrigger(DieHash);

        StartCoroutine(DeathSequence(Category));
    }

    private IEnumerator DeathSequence(string Category)
    {
        yield return new WaitForSeconds(1f);
        gameObject.layer = LayerMask.NameToLayer("Ghost");

        if (playerSprite != null)
        {
            var sr = playerSprite.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                var c = sr.color;
                c.a = ghostAlpha;
                sr.color = c;
            }
        }

        if (rb != null)
            rb.simulated = false;

        if (Category != "vote")
        {
            DeadBodyManager.Instance.SpawnDeadBody(transform.position, PhotonNetwork.LocalPlayer.ActorNumber);    
        }
        
        isGhost = true;
    }
    public void OnDanceInput(InputAction.CallbackContext ctx)   // Input System에서 G 키와 매핑
    {
        if (photonView.IsMine && ctx.performed)
        {
            playDance = true;                     // 내 화면 즉시 트리거
            animator.SetTrigger("Dance");         // Animator에 Dance 트리거

            // 다른 클라이언트에도 알려주기 (간단 RPC)
            photonView.RPC(nameof(DoDanceRPC), RpcTarget.Others);
        }
    }

    [PunRPC] void DoDanceRPC()
    {
        animator.SetTrigger("Dance");
    }

}
