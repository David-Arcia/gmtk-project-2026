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
    public float downRayDetectionSize = 0.05f;
    [SerializeField]
    public float sideRayDetectionSize = 0.06f;
    [SerializeField]
    public float downRaySpacing = 0.03f;
    [SerializeField]
    public float sideRaySpacing = 0.04f;
    [SerializeField]
    public float damageKnockbackForce = 12f;
    [SerializeField]
    public float iFrameLength = 0.5f;
    [SerializeField]
    public float damagedStateLength = 1f;
    [SerializeField]
    public Vector2 DefaultResetPosition;
    [SerializeField]
    public float killHeight = -0.5f;
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
    private float damagedStateCounter;
    private bool isDamaged;
    private bool isInIFrame;
    private float iFrameCounter;
    private DeathParticleSpawner particleSpawner;
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
        particleSpawner = GetComponent<DeathParticleSpawner>();
        prevPlayerPos = Vector2.zero;
        jumpCounter = 0f;
        jumpOnCooldown = false;
        endScreenShown = false;
        damagedStateCounter = 0f;
        isDamaged = false;
        isInIFrame = false;
        iFrameCounter = 0;
        if (enterDoor)
        {
            rb.position = new Vector2(enterDoor.transform.position.x, enterDoor.transform.position.y + enterDoorOffset);
        }
        else
        {
            rb.position = DefaultResetPosition;
        }
        Time.timeScale = 1f;

    }

    // Update is called once per frame
    void Update()
    {
        PlayerPosition = rb.position;
        if (PlayerPosition.y < killHeight)
        {
            energyController.EnergyAmount = 0;
        }
        UpdateDamagedState();
        UpdateIFrame();
        UpdateDash();
        UpdateJump();
        if (currGroundingState == GroundingStates.WALL_CLING)
        {
            GroundingRaycastChecks();
        }
        /*if (prevGroundedState != currGroundingState)
        {
            Debug.Log("Current GroundingState:" + currGroundingState);
        }*/

        if (!isDashing && !isDamaged)
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
        if (!isDamaged)
        {

            spriteAnimator.SetDashOnCooldown(!CanDash());
        }

    }

    void OnCollisionStay2D(Collision2D collision)
    {
        ColliderEnterExitCheck(collision);
        Collider2D collider = collision.collider;
        if (collider.tag == "Spike")
        {
            ApplyDamageImpulse(collider.gameObject.transform.position);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        ColliderEnterExitCheck(collision);
    }

    void ColliderEnterExitCheck(Collision2D collision)
    {
        if (collision.collider.tag == "Terrain")
        {
            GroundingRaycastChecks();
        }
    }

    void GroundingRaycastChecks()
    {
        //Shoot rays to check which sides have been touched
        RaycastHit2D[] hitsDown1 = Physics2D.RaycastAll(PlayerPosition + new Vector2(downRaySpacing, 0), Vector2.down, downRayDetectionSize);
        RaycastHit2D[] hitsDown2 = Physics2D.RaycastAll(PlayerPosition + new Vector2(-downRaySpacing, 0), Vector2.down, downRayDetectionSize);
        RaycastHit2D[] hitsRight1 = Physics2D.RaycastAll(PlayerPosition + new Vector2(0, sideRaySpacing), Vector2.right, sideRayDetectionSize);
        RaycastHit2D[] hitsLeft1 = Physics2D.RaycastAll(PlayerPosition + new Vector2(0, sideRaySpacing), Vector2.left, sideRayDetectionSize);
        RaycastHit2D[] hitsRight2 = Physics2D.RaycastAll(PlayerPosition + new Vector2(0, -sideRaySpacing), Vector2.right, sideRayDetectionSize);
        RaycastHit2D[] hitsLeft2 = Physics2D.RaycastAll(PlayerPosition + new Vector2(0, -sideRaySpacing), Vector2.left, sideRayDetectionSize);

        if (CheckHitArrayForTag(hitsDown1, "Terrain") || CheckHitArrayForTag(hitsDown2, "Terrain"))
        {
            currGroundingState = GroundingStates.GROUNDED;
        }
        else if (CheckHitArrayForTag(hitsRight1, "Terrain") || CheckHitArrayForTag(hitsLeft1, "Terrain") || CheckHitArrayForTag(hitsRight2, "Terrain") || CheckHitArrayForTag(hitsLeft2, "Terrain"))
        {
            currGroundingState = GroundingStates.WALL_CLING;
        }
        else
        {
            currGroundingState = GroundingStates.AERIAL;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        if (collider.tag == "Spike")
        {
            ApplyDamageImpulse(collider.gameObject.transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            if (isDashing && !IsTouchingInBetweenSpike(collider))
            {
                KillEnemy(collider);
            }
            else
            {
                ApplyDamageImpulse(collider.gameObject.transform.position);
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
        rb.linearVelocity = Vector2.zero;
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
        return !dashOnCooldown && !dashUsedSinceLastAnchoring;
    }

    private bool ShouldDashEnd()
    {
        return dashIframesEnded || isDamaged;
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
        if (currGroundingState != GroundingStates.AERIAL && !isDashing)
        {
            dashUsedSinceLastAnchoring = false;
        }
        if (CanDash() && inputController.PressedDash && !isDamaged)
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

    private void ApplyDamageImpulse(Vector2 enemyLocation)
    {
        if (isInIFrame)
        {
            return;
        }
        //Knock away and set damaged state
        isDamaged = true;
        spriteAnimator.SetDamaged(true);
        Vector2 knockBackVector = (PlayerPosition - enemyLocation).normalized * damageKnockbackForce;
        energyController.RemoveEnergyFromCollisionWithFoe();
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockBackVector);
        audioController.PlayAudio(AudioEffects.HURT);
        isInIFrame = true;
    }

    private void KillEnemy(Collider2D collider)
    {
        energyController.AddEnergyFromFallenFoe();
        bool isRechargeEnemy = false;
        if (collider.gameObject.GetComponent<RechargeDashEnemy>())
        {
            isRechargeEnemy = true;
            ResetDash();
        }
        Destroy(collider.gameObject);
        isInIFrame = true;
        particleSpawner.RequestParticleEffect(collider.gameObject.transform.position, isRechargeEnemy);
        audioController.PlayAudio(AudioEffects.ENEMY_DEATH);
    }

    private void UpdateDamagedState()
    {
        if (isDamaged)
        {
            damagedStateCounter += Time.deltaTime;
            if (damagedStateCounter >= damagedStateLength)
            {
                isDamaged = false;
                spriteAnimator.SetDamaged(false);
            }
        }
        else
        {
            damagedStateCounter = 0;
        }
    }

    private Vector2? IsTouchingSpike()
    {
        ContactFilter2D filter = ContactFilter2D.noFilter;
        Collider2D[] results = new Collider2D[10];
        int overlapCount = rb.Overlap(filter, results);
        Vector2? output = null;
        if (overlapCount > 0)
        {
            for (int i = 0; i < overlapCount; i++)
            {
                if (results[i].tag == "Spike")
                {
                    output = results[i].gameObject.transform.position;
                }
            }
        }
        return output;
    }

    private bool IsTouchingInBetweenSpike(Collider2D collider)
    {
        bool output = false;
        Vector2? potentialSpike = IsTouchingSpike();
        if (potentialSpike.HasValue)
        {
            Vector2 nonNullPotentialSpike = potentialSpike ?? Vector2.zero;
            if (Vector2.Distance(nonNullPotentialSpike, rb.position) < Vector2.Distance(collider.gameObject.transform.position, rb.position))
            {
                output = true;
            }
        }
        return output;
    }

    private void ResetDash()
    {
        dashUsedSinceLastAnchoring = false;
        dashOnCooldown = false;
    }

    private void UpdateIFrame()
    {
        if (isInIFrame)
        {
            iFrameCounter += Time.deltaTime;
            if (iFrameCounter >= iFrameLength)
            {
                isInIFrame = false;
            }
        }
        else
        {
            iFrameCounter = 0;
        }
    }
}
