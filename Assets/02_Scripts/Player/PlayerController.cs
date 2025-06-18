using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
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
    
    [Header("State")]
    public bool IsInteraction = false;
    
    [Header("Ghost Settings")]
    [SerializeField] private float ghostMoveSpeed = 6f;
    [SerializeField] private float ghostAlpha = 0.5f;
    private bool isGhost = false;

    public TMP_Text playerName;
    
    private bool IsUIFocused()
    {
        var sel = EventSystem.current?.currentSelectedGameObject;
        if (sel == null) return false;
        return sel.GetComponent<TMP_InputField>() != null;

    }
    
    public void SetInteraction(bool on)
    {
        IsInteraction = on;                        // on == true → 잠금
        PlayerUIManager.Instance?.
            SetUseButtonInteractable(!on);         // 버튼은 잠금 시 비활성

        if (on && rb != null)                      // 잠금이면 즉시 정지
            rb.velocity = Vector2.zero;
    }

    
    private float killCooldown = 0f;


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
        SetName();
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
    
    private void SetName()
    {
        Debug.Log("Set name");
        Player player = photonView.Owner;
        playerName.text = player.NickName;
    }

    private void FixedUpdate()
    {
        if (IsInteraction)        // 잠금 상태면 아무것도 못 함
        {
            rb.velocity = Vector2.zero;
            return;
        }
        
        if (killCooldown > 0)
        {
            killCooldown -= Time.fixedDeltaTime;
        }
        
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
        if (IsInteraction || IsUIFocused()) 
            return;
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnInteractInput(InputAction.CallbackContext context)
    {
        if (IsInteraction || !photonView.IsMine || !context.performed)
            return;
        OnInteractAction();
        
    }

    public void OnInteractAction()
    {
        
        Collider2D target = Physics2D.OverlapCircle(transform.position, interactRange, interactLayer);
        if (target != null) OnInteract?.Invoke();
    }

    public void OnKillInput(InputAction.CallbackContext context)
    {
        if (isGhost || IsInteraction || !photonView.IsMine || !context.performed)
            return;
        TryKill();
        
    }

    public void TryKill()
    {
        Debug.Log(killCooldown);
        if (killCooldown > 0) return;
        if (photonView.gameObject.TryGetComponent<ImposterController>(out ImposterController imposter))
        {
            imposter.TryKill();    
        }
        else
        {
            Debug.Log("This Object is Not Imposter");
        }
    }

    public void SetKillCooldown(float cooldown)
    {
        this.killCooldown = cooldown;
        StartCoroutine(PlayerUIManager.Instance.SetKillButtonCooldown(cooldown));
    }

    public void OnJumpInput(InputAction.CallbackContext ctx)
    {
        if (IsInteraction || IsUIFocused() || !photonView.IsMine || !ctx.performed) 
            return;

        PhotonNetwork.RaiseEvent(
            EventCodes.PlayerJump,
            new object[] { PhotonNetwork.LocalPlayer.ActorNumber },
            new RaiseEventOptions { Receivers = ReceiverGroup.All },
            SendOptions.SendReliable);
    }

    public IEnumerator JumpCoroutine()
    {
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
        if (IsInteraction || !photonView.IsMine || !context.performed) 
            return;

        OnOpenInventory?.Invoke();
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
        if (IsInteraction || IsUIFocused() || !photonView.IsMine || !context.performed) 
            return;

        OnReportAction();
    }

    public void OnReportAction()
    {
        if (isGhost) return;
        int deadBodyActorNum = DeadBodyManager.Instance.GetClosestDeadBodyID(transform.position);
        if (deadBodyActorNum == 0) return;
        ReportManager.Instance.ReportBody(deadBodyActorNum);
    }
    public void Die(string category = "killing")
    {
        animator?.SetTrigger(DieHash);
        StartCoroutine(DeathSequence(category));
    }

    private IEnumerator DeathSequence(string category)
    {
        yield return new WaitForSeconds(1f);

        // 1) 레이어 재귀 변경
        int ghostLayer = LayerMask.NameToLayer("Ghost");
        SetLayerRecursively(gameObject, ghostLayer);

        // 2) 카메라 전환은 **본인**만
        if (photonView.IsMine)
            GetComponent<LayerController>()?.SwitchToGhostView();

        // 3) 반투명 처리 (본인 + 원격 모두)
        if (playerSprite != null)
        {
            var sr = playerSprite.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;  c.a = ghostAlpha;
                sr.color = c;
            }
        }

        // 4) 물리 중지
        if (rb != null)
            rb.simulated = false;

        // 5) 시체 생성(투표 사망은 제외)
        if (category != "vote")
            DeadBodyManager.Instance.SpawnDeadBody(transform.position,
                PhotonNetwork.LocalPlayer.ActorNumber);

        isGhost = true;

        if (photonView.IsMine)
        {
            MissionManager.Instance.CloseAllMissionUIs();
        }
    }
    private static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform t in obj.transform)
            SetLayerRecursively(t.gameObject, layer);
    }

    public void OnDanceInput(InputAction.CallbackContext ctx)   // Input System에서 G 키와 매핑
    {
        if (IsInteraction || !photonView.IsMine || !ctx.performed) 
            return;

        playDance = true;
        animator.SetTrigger("Dance");
        photonView.RPC(nameof(DoDanceRPC), RpcTarget.Others);
    }

    [PunRPC] void DoDanceRPC()
    {
        animator.SetTrigger("Dance");
    }

}
