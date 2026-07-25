using UnityEngine;

public class FloatUpAndDown : MonoBehaviour
{
    [SerializeField]
    public float amplidtude = 1;
    [SerializeField]
    public float frequency = 1;
    [SerializeField]
    public bool startReversed = false;
    private Vector2 initPosition;
    private Rigidbody2D rb;
    private float initTime;
    private float reverseMultiplier;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initPosition = gameObject.transform.position;
        rb = GetComponent<Rigidbody2D>();
        initTime = Time.time;
        if (startReversed)
        {
            reverseMultiplier = -1;
        } else
        {
            reverseMultiplier = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float newY = Mathf.Sin((Time.time-initTime)*frequency) * amplidtude * reverseMultiplier;
        rb.MovePosition(initPosition + new Vector2(0, newY));
    }
}
