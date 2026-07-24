using UnityEngine;
using UnityEngine.InputSystem;

public class Input : MonoBehaviour
{

    [SerializeField]
    public Camera camera;
    [SerializeField]
    private InputActionAsset inputAction;
    [SerializeField]
    private string playerInputMapName;
    [SerializeField]
    private string playerMoveName = "Move";
    [SerializeField]
    private string playerAimName = "Aim";
    [SerializeField]
    private string playerDashName = "Dash";
    [SerializeField]
    private string playerResetName = "Reset";
    public Vector2 AimPosition { get; set; }
    public Vector2 MoveVector { get; set; }
    public bool PressedDash { get; set; }
    public bool PressedReset { get; set; }
    public bool PressedJump { get; set; }
    private bool isPressingDash = false;
    private bool onDashCoolDown = false;
    private bool isPressingReset = false;
    private bool onResetCoolDown = false;
    private bool isPressingJump = false;
    private bool onJumpCooldown = false;
    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction dashAction;
    private InputAction resetAction;
    [SerializeField]
    public float dashInputCooldown = 0.25f;
    [SerializeField]
    public float resetInputCooldown = 0.25f;
    [SerializeField]
    public float jumpInputCooldown = 0.25f;
    private float dashTimer = 0f;
    private float resetTimer = 0f;
    private float jumpTimer = 0f;

    void Start()
    {

        moveAction = inputAction.FindActionMap(playerInputMapName).FindAction(playerMoveName);
        aimAction = inputAction.FindActionMap(playerInputMapName).FindAction(playerAimName);
        dashAction = inputAction.FindActionMap(playerInputMapName).FindAction(playerDashName);
        resetAction = inputAction.FindActionMap(playerInputMapName).FindAction(playerResetName);

        RegisterInputActions();
        PressedDash = false;
        PressedReset = false;
        PressedJump = false;
    }

    void RegisterInputActions()
    {
        dashAction.performed += context => isPressingDash = true;
        dashAction.canceled += context => isPressingDash = false;
        dashAction.Enable();

        resetAction.performed += context => isPressingReset = true;
        resetAction.canceled += context => isPressingReset = false;
        resetAction.Enable();

        dashAction.performed += context => isPressingDash = true;
        dashAction.canceled += context => isPressingDash = false;
        dashAction.Enable();

        aimAction.Enable();

        moveAction.performed += context => MoveVector = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveVector = Vector2.zero;
        moveAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 aimPos = aimAction.ReadValue<Vector2>();
        AimPosition = camera.ScreenToWorldPoint(new Vector3(aimPos.x, aimPos.y, -(camera.gameObject.transform.position.z)));
        isPressingJump = MoveVector.y > 0;

        if (onDashCoolDown)
        {
            PressedDash = false;
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashInputCooldown)
            {
                dashTimer = 0;
                onDashCoolDown = false;
            }
        }
        else
        {
            if (isPressingDash)
            {
                PressedDash = true;
                onDashCoolDown = true;
            }
        }

        if (onResetCoolDown)
        {
            PressedReset = false;
            resetTimer += Time.deltaTime;
            if (resetTimer >= resetInputCooldown)
            {
                resetTimer = 0;
                onResetCoolDown = false;
            }
        }
        else
        {
            if (isPressingReset)
            {
                PressedReset = true;
                onResetCoolDown = true;
            }
        }

        if (onJumpCooldown)
        {
            PressedJump = false;
            jumpTimer += Time.deltaTime;
            if (jumpTimer >= jumpInputCooldown)
            {
                jumpTimer = 0;
                onJumpCooldown = false;
            }
        }
        else
        {
            if (isPressingJump)
            {
                PressedJump = true;
                onJumpCooldown = true;
            }
        }
    }
}
