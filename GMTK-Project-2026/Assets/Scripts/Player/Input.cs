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
    private bool isPressingDash = false;
    private bool onDashCoolDown = false;
    private bool isPressingReset = false;
    private bool onResetCoolDown = false;
    private InputAction moveAction;
    private InputAction aimAction;
    private InputAction dashAction;
    private InputAction resetAction;
    [SerializeField]
    public float dashCooldown = 1f;
    [SerializeField]
    public float resetCooldown = 1f;
    private float dashTimer = 0f;
    private float resetTimer = 0f;

    void Start()
    {

        moveAction = inputAction.FindActionMap(playerInputMapName).FindAction(playerMoveName);
        aimAction = inputAction.FindActionMap(playerInputMapName).FindAction(playerAimName);
        dashAction = inputAction.FindActionMap(playerInputMapName).FindAction(playerDashName);
        resetAction = inputAction.FindActionMap(playerInputMapName).FindAction(playerResetName);

        RegisterInputActions();
        PressedDash = false;
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
        AimPosition = camera.ScreenToWorldPoint(aimAction.ReadValue<Vector2>());
        if (onDashCoolDown)
        {
            PressedDash = false;
            dashTimer += Time.deltaTime;
            if (dashTimer >= dashCooldown)
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
            if (resetTimer >= resetCooldown)
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
    }
}
