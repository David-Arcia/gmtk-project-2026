using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    public Sprite neutralSprite;
    [SerializeField]
    public Sprite groundedDirectionalSprite;
    [SerializeField]
    public Sprite aerialSprite;
    [SerializeField]
    public int numGhostSprites = 4;
    [SerializeField]
    public float ghostSpriteSpacingMultiplier = 1;
    [SerializeField]
    public GameObject ghostrSpritePrefab;
    private GameObject[] ghostSpriteObjects;
    private SpriteRenderer renderer;
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        ghostSpriteObjects = new GameObject[numGhostSprites];
        for (int i = 0; i < numGhostSprites; i++)
        {
            GameObject newObj = Instantiate(ghostrSpritePrefab, Vector2.zero, Quaternion.identity);
            newObj.SetActive(false);
            ghostSpriteObjects[i] = newObj;
        }
    }

    public void SetRigidBody(Rigidbody2D rb)
    {
        this.rb = rb;
    }

    void Update()
    {
        if (rb)
        {
            Vector2 playerVelo = rb.linearVelocity;
            float veloMag = playerVelo.magnitude;
            for (int i = 0; i < numGhostSprites; i++)
            {
                GameObject obj = ghostSpriteObjects[i];
                obj.transform.position = rb.position + -(i + 1) * ghostSpriteSpacingMultiplier * veloMag * playerVelo.normalized;
                SpriteRenderer ghostRenderer = obj.GetComponent<SpriteRenderer>();
                if (ghostRenderer)
                {
                    if (playerVelo.x < 0)
                    {
                        ghostRenderer.flipX = true;
                    }
                    else
                    {
                        ghostRenderer.flipX = false;
                    }
                }
            }
        }
    }

    public void DrawGhostSprites(bool shouldDraw)
    {
        foreach (GameObject obj in ghostSpriteObjects)
        {
            obj.SetActive(shouldDraw);
        }
    }

    public void SetNeutralSprite()
    {
        renderer.sprite = neutralSprite;
    }

    public void SetGroundedDirSprite(bool flip)
    {
        renderer.sprite = groundedDirectionalSprite;
        renderer.flipX = flip;
    }

    public void SetAerialSprite(bool flip)
    {
        renderer.sprite = aerialSprite;
        renderer.flipX = flip;
    }
}
