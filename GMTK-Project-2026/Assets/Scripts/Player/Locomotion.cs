using UnityEngine;
using TMPro;

public enum GroundingStates
{
    GROUNDED,
    WALL_CLING,
    AERIAL
}

public class Locomotion : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 1;
    [SerializeField]
    public float dashImpulse = 1;
    [SerializeField]
    public float dashCooldown = 0.5f;
    [SerializeField]
    public float dashLength = 0.25f;
    [SerializeField]
    public float jumpImpulse = 1;
    [SerializeField]
    public float jumpCooldown = 0.1f;
    [SerializeField]
    public float rayDetectionSize = 0.05f;
    [SerializeField]
    public float damageKnockbackMultiplier = 0.75f;
    [SerializeField]
    public Vector2 DefaultResetPosition;
    [SerializeField]
    public float enterDoorOffset = 0.04f;
    [SerializeField]
    public GameObject levelCompleteScreen;
    [SerializeField]
    public TextMeshProUGUI resultText;
    private Input inputController;
    private bool isDashing;
    private bool touchingWall;
    private bool touchingGround;
    public Vector2 PlayerPosition { get; set; } = Vector2.zero;
    private Rigidbody2D rb;
    private EnergyController energyController;
    private SpriteAnimator spriteAnimator;
    private GameObject enterDoor;
    private GroundingStates currGroundingState;
    private GroundingStates prevGroundedState;
    private bool dashUsedSinceLastAnchoring;
    private bool dashOnCooldown;
    private bool dashIframesEnded;
    private float dashCooldownCounter;
    private float dashLengthCounter;
    private Vector2 prevPlayerPos;
    private float jumpCounter;
    private bool jumpOnCooldown;
    private PlayerAudioController audioController;
    private bool endScreenShown;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currGroundingState = GroundingStates.GROUNDED;
        prevGroundedState = GroundingStates.GROUNDED;
        touchingWall = false;
        touchingGround = false;
        isDashing = false;
        dashUsedSinceLastAnchoring = false;
        dashOnCooldown = false;
        dashIframesEnded = false;
        dashCooldownCounter = 0f;
        dashLengthCounter = 0f;
        inputController = GetComponent<Input>();
        rb = GetComponent<Rigidbody2D>();
        energyController = GetComponent<EnergyController>();
        spriteAnimator = GetComponent<SpriteAnimator>();
        spriteAnimator.SetRigidBody(rb);
        enterDoor = GameObject.Find("EnterDoor");
        audioController = GetComponent<PlayerAudioController>();
        prevPlayerPos = Vector2.zero;
        jumpCounter = 0f;
        jumpOnCooldown = false;
        endScreenShown = false;
        if (enterDoor)
        {
            rb.position = new Vector2(enterDoor.transform.position.x, enterDoor.transform.position.y + enterDoorOffset);
        }
        else
        {
            rb.position = DefaultResetPosition;
        }

    }

    // Update is called once per frame
    void Update()
    {
        PlayerPosition = rb.position;
        UpdateDash();
        UpdateJump();
        /*if (prevGroundedState != currGroundingState)
        {
            Debug.Log("Current GroundingState:" + currGroundingState);
        }*/

        if (!isDashing)
        {
            //Handle AD movement
            if (currGroundingState == GroundingStates.WALL_CLING)
            {
                rb.linearVelocity = Vector2.zero;
            }

            rb.linearVelocity = new Vector2(inputController.MoveVector.x * moveSpeed, rb.linearVelocity.y);
            //Handle jump
            if (currGroundingState == GroundingStates.GROUNDED && inputController.PressedJump && !jumpOnCooldown)
            {
                rb.AddForce(Vector2.up * jumpImpulse);
                touchingGround = false;
                jumpOnCooldown = true;
            }

        }
        if (energyController.EnergyAmount == 0 && !endScreenShown)
        {
            ShowEndScreen(false);
            endScreenShown = true;
        }
        prevGroundedState = currGroundingState;
        prevPlayerPos = PlayerPosition;

        //Animatesprites
        float xdir = rb.linearVelocity.x;
        bool flip = xdir < 0;
        if (isDashing)
        {
            spriteAnimator.SetAerialSprite(flip);
        }
        else
        {
            if (Mathf.Abs(xdir) > 0.001)
            {
                spriteAnimator.SetGroundedDirSprite(flip);
            }
            else
            {
                spriteAnimator.SetNeutralSprite();
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ColliderEnterExitCheck(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        ColliderEnterExitCheck(collision);
    }

    void ColliderEnterExitCheck(Collision2D collision)
    {
        if (collision.collider.tag == "Terrain")
        {

            //Shoot rays to check which sides have been touched
            RaycastHit2D[] hitsDown = Physics2D.RaycastAll(PlayerPosition, Vector2.down, rayDetectionSize);
            RaycastHit2D[] hitsRight = Physics2D.RaycastAll(PlayerPosition, Vector2.right, rayDetectionSize);
            RaycastHit2D[] hitsLeft = Physics2D.RaycastAll(PlayerPosition, Vector2.left, rayDetectionSize);

            if (CheckHitArrayForTag(hitsDown, "Terrain"))
            {
                currGroundingState = GroundingStates.GROUNDED;
            }
            else if (CheckHitArrayForTag(hitsRight, "Terrain") || CheckHitArrayForTag(hitsLeft, "Terrain"))
            {
                currGroundingState = GroundingStates.WALL_CLING;
            }
            else
            {
                currGroundingState = GroundingStates.AERIAL;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Spike")
        {
            energyController.RemoveEnergyFromCollisionWithFoe();
            ApplyDamageImpulse();
        }
        else if (collider.tag == "Enemy")
        {
            if (isDashing)
            {
                energyController.AddEnergyFromFallenFoe();
                Destroy(collider.gameObject);
            }
            else
            {
                energyController.RemoveEnergyFromCollisionWithFoe();
                ApplyDamageImpulse();
            }
        }
        else if (collider.tag == "Exit")
        {
            ShowEndScreen(true);
        }
    }

    private void ShowEndScreen(bool hasWon)
    {
        levelCompleteScreen.SetActive(true);
        Time.timeScale = 0f;
        if (hasWon)
        {
            resultText.text = "YOU WIN!";
        }
        else
        {
            resultText.text = "YOU LOSE";
        }
    }

    private void HandleGroundingState()
    {
        if (touchingGround)
        {
            currGroundingState = GroundingStates.GROUNDED;
        }
        else if (touchingWall)
        {
            currGroundingState = GroundingStates.WALL_CLING;
        }
        else
        {
            currGroundingState = GroundingStates.AERIAL;
        }
    }

    private bool CheckHitArrayForTag(RaycastHit2D[] array, string tag)
    {
        bool output = false;
        foreach (RaycastHit2D hit in array)
        {
            if (hit.collider.tag == tag)
            {
                output = true;
            }
        }
        return output;
    }

    private void StartDash(Vector2 direction)
    {
        isDashing = true;
        rb.gravityScale = 0;
        rb.AddForce(direction.normalized * dashImpulse);
        dashOnCooldown = true;
        dashUsedSinceLastAnchoring = true;
        currGroundingState = GroundingStates.AERIAL;
        spriteAnimator.DrawGhostSprites(true);
        audioController.PlayAudio(AudioEffects.DASH);
    }

    private void EndDash()
    {
        isDashing = false;
        rb.gravityScale = 1;
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        spriteAnimator.DrawGhostSprites(false);
    }

    private bool CanDash()
    {
        return !isDashing && !dashOnCooldown && !dashUsedSinceLastAnchoring;
    }

    private bool ShouldDashEnd()
    {
        return dashIframesEnded;
    }

    private void HandleDashCooldowns()
    {
        if (dashOnCooldown)
        {
            dashCooldownCounter += Time.deltaTime;
            if (dashCooldownCounter >= dashCooldown)
            {
                dashOnCooldown = false;
            }
        }
        else
        {
            dashCooldownCounter = 0;
        }

        if (isDashing)
        {
            dashLengthCounter += Time.deltaTime;
            if (dashLengthCounter >= dashLength)
            {
                dashIframesEnded = true;
            }
        }
        else
        {
            dashLengthCounter = 0;
        }
    }

    private void UpdateDash()
    {
        HandleDashCooldowns();
        if (currGroundingState != GroundingStates.AERIAL)
        {
            dashUsedSinceLastAnchoring = false;
        }
        if (CanDash() && inputController.PressedDash)
        {
            StartDash(inputController.AimPosition - PlayerPosition);
        }
        else if (ShouldDashEnd())
        {
            EndDash();
            dashIframesEnded = false;
        }
    }

    private void UpdateJump()
    {
        if (jumpOnCooldown)
        {
            jumpCounter += Time.deltaTime;
            if (jumpCounter >= jumpCooldown)
            {
                jumpOnCooldown = false;
            }
        }
        else
        {
            jumpCounter = 0;
        }
    }
    
    private void ApplyDamageImpulse()
    {
        //Knock away from damage source
    }
}
