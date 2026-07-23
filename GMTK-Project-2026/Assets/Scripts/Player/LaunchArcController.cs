using UnityEngine;

public class LaunchArcController : MonoBehaviour
{
    [SerializeField]
    public int numPoints = 10;
    [SerializeField]
    public float dashImpulse = 5f;
    [SerializeField]
    public float timeStep = 0.1f;
    [SerializeField]
    public int redrawCooldown = 10;
    private int redrawCounter = 0;
    private Vector2 prevRBPos = Vector2.zero;
    private LineRenderer lineRenderer;
    private Rigidbody2D rb;
    private Input inputController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        inputController = GetComponent<Input>();

        lineRenderer.positionCount = numPoints;
    }

    // Update is called once per frame
    void Update()
    {
        redrawCounter++;
        if (redrawCounter >= redrawCooldown)
        {
            Vector2 predictedDashVector = (inputController.AimPosition - rb.position).normalized * dashImpulse;
            PredictPath(rb, predictedDashVector);
            redrawCounter = 0;
        }
    }

    public void EnableLineRender(bool enabled)
    {
        lineRenderer.enabled = enabled;
    }

    public void PredictPath(Rigidbody2D rb, Vector2 impulseForce)
    {
        // 1. Calculate the instantaneous change in velocity caused by the impulse
        Vector3 initialVelocity = rb.linearVelocity + (impulseForce / rb.mass);
        Vector3 startPosition = rb.position;
        Vector3 gravity = Physics.gravity;

        bool isBlocked = false;
        Vector2 lastPlottedPoint = Vector2.zero;

        // 2. Plot points along the kinematic curve
        for (int i = 0; i < numPoints; i++)
        {
            if (isBlocked)
            {
                lineRenderer.SetPosition(i, lastPlottedPoint);
                continue;
            }
            float t = i * timeStep;
            // Formula: s = s0 + v0*t + 0.5*g*t^2
            Vector2 predictedPosition = startPosition + (initialVelocity * t) + (0.5f * gravity * t * t);
            if (i > 0)
            {
                //check if hitting something
                RaycastHit2D hit = Physics2D.Raycast(lastPlottedPoint, (predictedPosition-lastPlottedPoint).normalized, Vector2.Distance(predictedPosition, lastPlottedPoint));
                if (hit)
                {
                    if (hit.collider.tag == "Terrain")
                    {
                        predictedPosition = hit.point;
                        isBlocked = true;
                    }
                }
            }

            lineRenderer.SetPosition(i, predictedPosition);
            lastPlottedPoint = predictedPosition;
        }
    }
}
