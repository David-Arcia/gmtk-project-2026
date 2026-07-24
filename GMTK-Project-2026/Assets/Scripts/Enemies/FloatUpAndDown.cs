using UnityEngine;

public class FloatUpAndDown : MonoBehaviour
{
    [SerializeField]
    public float amplidtude = 1;
    [SerializeField]
    public float frequency = 1;
    private Vector2 initPosition;
    private Rigidbody2D rb;
    private float initTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initPosition = gameObject.transform.position;
        rb = GetComponent<Rigidbody2D>();
        initTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float newY = Mathf.Sin((Time.time-initTime)*frequency) * amplidtude;
        rb.MovePosition(initPosition + new Vector2(0, newY));
    }
}
