using UnityEngine;

public class HorizontalMovementSpikeManager : MonoBehaviour
{
    [SerializeField]
    public GameObject spikeL;
    [SerializeField]
    public GameObject spikeR;
    private Rigidbody2D rb;
    private float lastPosX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spikeL.SetActive(false);
        spikeR.SetActive(true);
        rb = GetComponent<Rigidbody2D>();
        lastPosX = rb.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (rb.position.x - lastPosX >= 0)
        {
            spikeL.SetActive(false);
            spikeR.SetActive(true);
        }
        else
        {
            spikeL.SetActive(true);
            spikeR.SetActive(false);
        }
        lastPosX = rb.position.x;
    }
}
