using UnityEngine;
using TMPro;

public class Locomotion : MonoBehaviour
{
    [SerializeField]
    public float moveSpeed = 1;
    [SerializeField]
    public float dashImpulse = 1;
    [SerializeField]
    public float rayDetectionSize = 0.05f;
    [SerializeField]
    public Vector2 DefaultResetPosition;
    [SerializeField]
    public float enterDoorOffset = 0.04f;
    [SerializeField]
    public GameObject levelCompleteScreen;
    [SerializeField]
    public TextMeshProUGUI resultText;
    private Input inputController;
    private bool isDashing = false;
    private bool touchingWall = false;
    private bool touchingGround = false;
    public Vector2 PlayerPosition { get; set; } = Vector2.zero;
    private Rigidbody2D rb;
    private EnergyController energyController;
    private SpriteAnimator spriteAnimator;
    private LaunchArcController launchRenderer;
    private GameObject enterDoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inputController = GetComponent<Input>();
        rb = GetComponent<Rigidbody2D>();
        energyController = GetComponent<EnergyController>();
        spriteAnimator = GetComponent<SpriteAnimator>();
        launchRenderer = GetComponent<LaunchArcController>();
        enterDoor = GameObject.Find("EnterDoor");
        if (enterDoor)
        {
            rb.position = new Vector2(enterDoor.transform.position.x, enterDoor.transform.position.y + enterDoorOffset);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //Sanity check
        if (isDashing && Mathf.Abs(rb.linearVelocity.magnitude) < 0.01)
        {
            isDashing = false;
        }
        PlayerPosition = gameObject.transform.position;
        if (touchingGround || touchingWall)
        {
            isDashing = false;
            launchRenderer.EnableLineRender(true);
        }
        if (inputController.PressedDash && !isDashing)
        {
            isDashing = true;
            Vector2 dashVector = (inputController.AimPosition - PlayerPosition).normalized * dashImpulse;
            touchingGround = false;
            touchingWall = false;
            rb.AddForce(dashVector, ForceMode2D.Impulse);
            launchRenderer.EnableLineRender(false);
        }
        if (!isDashing && Mathf.Abs(inputController.MoveVector.x) > 0.001 && !inputController.PressedDash)
        {
            rb.linearVelocity = new Vector2(inputController.MoveVector.x * moveSpeed, rb.linearVelocity.y);
        }
        if (Mathf.Abs(rb.linearVelocity.y) > 0.01)
        {
            if (rb.linearVelocity.x > 0.01)
            {
                spriteAnimator.SetAerialSprite(false);
            }
            else
            {
                spriteAnimator.SetAerialSprite(true);
            }
        }
        else
        {
            if (rb.linearVelocity.x > 0.01)
            {
                spriteAnimator.SetGroundedDirSprite(false);
            }
            else if (rb.linearVelocity.x < -0.01)
            {
                spriteAnimator.SetGroundedDirSprite(true);
            } else
            {
                spriteAnimator.SetNeutralSprite();
            }
        }

        if (energyController.EnergyAmount == 0)
        {
            ShowEndScreen(false);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Terrain")
        {

            //Shoot rays to check which sides have been touched
            RaycastHit2D hitDown = Physics2D.Raycast(PlayerPosition, Vector2.down, rayDetectionSize);
            RaycastHit2D hitRight = Physics2D.Raycast(PlayerPosition, Vector2.right, rayDetectionSize);
            RaycastHit2D hitLeft = Physics2D.Raycast(PlayerPosition, Vector2.left, rayDetectionSize);

            if (hitDown)
            {
                touchingGround = true;
            }
            else
            {
                touchingGround = false;
            }

            if (hitRight || hitLeft)
            {
                touchingWall = true;
            }
            else
            {
                touchingWall = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "Enemy")
        {
            if (isDashing)
            {
                energyController.AddEnergyFromFallenFoe();
                Destroy(collider.gameObject);
            } else
            {
                energyController.RemoveEnergyFromCollisionWithFoe();
            }
        }
        if (collider.tag == "Exit")
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
        } else
        {
            resultText.text = "YOU LOSE";
        }
    }
}
