using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    public float destroyTime = 1f;
    private float destroyCounter;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        destroyCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        destroyCounter+= Time.deltaTime;
        if (destroyCounter >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
}
