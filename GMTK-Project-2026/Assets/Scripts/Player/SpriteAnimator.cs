using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    [SerializeField]
    public Sprite neutralSprite;
    [SerializeField]
    public Sprite groundedDirectionalSprite;
    [SerializeField]
    public Sprite aerialSprite;
    private SpriteRenderer renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
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
