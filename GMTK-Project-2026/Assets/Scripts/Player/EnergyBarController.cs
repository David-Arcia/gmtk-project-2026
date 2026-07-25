using UnityEngine;

public class EnergyBarController : MonoBehaviour
{
    [SerializeField]
    public int segments = 10;
    [SerializeField]
    public float radius = 0.08f;
    [SerializeField]
    public float rotationOffset = 90f;
    [SerializeField]
    public LineRenderer renderer;
    private Rigidbody2D rb;
    private EnergyController energyController;
    private float targetRadians;
    private float radianStep;
    private int numPoints;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        energyController = GetComponent<EnergyController>();
        targetRadians = 2 * Mathf.PI;
        numPoints = segments + 1;
        renderer.positionCount = numPoints;
        radianStep = 2 * Mathf.PI / segments;
    }

    // Update is called once per frame
    void Update()
    {
        targetRadians = energyController.EnergyAmount / energyController.MaxEnergy * Mathf.PI * 2;
        for (int i = 0; i < numPoints; i++)
        {
            if (radianStep * i > targetRadians)
            {
                PlaceLinePoint(i, targetRadians);
                continue;
            }
            //Need final extra point to close circle;
            if (i == numPoints - 1)
            {
                float currRadian = radianStep * 0;
                PlaceLinePoint(i, currRadian);
            }
            else
            {
                float currRadian = radianStep * i;
                PlaceLinePoint(i, currRadian);
            }
        }
    }

    void PlaceLinePoint(int pos, float radians)
    {
        float xPos = radius * Mathf.Cos(radians);
        float yPos = radius * Mathf.Sin(radians);
        Vector2 unrotatedPos = rb.position + new Vector2(xPos, yPos);
        Quaternion rotation = Quaternion.Euler(0, 0, rotationOffset);
        Vector2 direction = unrotatedPos - rb.position;
        Vector2 rotationDirection = rotation * direction;
        renderer.SetPosition(pos, rb.position + rotationDirection);
    }
}
