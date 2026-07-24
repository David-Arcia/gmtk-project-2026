using UnityEngine;

public class SwapSpikeSide : MonoBehaviour
{
    [SerializeField]
    public GameObject spikeL;
    [SerializeField]
    public GameObject spikeR;
    [SerializeField]
    public float swapInterval;
    private float intervalCounter;
    [SerializeField]
    public bool startRight;
    private bool rightActive;
    private bool leftActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        intervalCounter = 0;
        if (startRight)
        {
            spikeL.SetActive(false);
            leftActive = false;
            spikeR.SetActive(true);
            rightActive = true;
        } else
        {
            spikeL.SetActive(true);
            leftActive = true;
            spikeR.SetActive(false);
            rightActive = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        intervalCounter+= Time.deltaTime;
        if (intervalCounter >= swapInterval)
        {
            ToggleSpikeSwap();
            intervalCounter = 0;
        }
    }

    void ToggleSpikeSwap()
    {
        rightActive = !rightActive;
        spikeR.SetActive(rightActive);
        leftActive = !leftActive;
        spikeL.SetActive(leftActive);
    }
}
