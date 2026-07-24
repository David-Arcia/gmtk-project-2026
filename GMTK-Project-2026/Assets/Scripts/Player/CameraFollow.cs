using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    public Camera camera;
    [SerializeField]
    public float minY;
    [SerializeField]
    public float maxY;
    [SerializeField]
    public float minX;
    [SerializeField]
    public float maxX;
    // Update is called once per frame
    void LateUpdate()
    {
        float xTarget = 0;
        if (gameObject.transform.position.x > minX)
        {
            if (gameObject.transform.position.x < maxX)
            {
                xTarget = gameObject.transform.position.x;
            } else
            {
                xTarget = maxX;
            }
        } else
        {
            xTarget = minX;
        }

        float yTarget = 0;
        if (gameObject.transform.position.y > minY)
        {
            if (gameObject.transform.position.y < maxY)
            {
                yTarget = gameObject.transform.position.y;
            } else
            {
                yTarget = maxY;
            }
        } else
        {
            yTarget = minY;
        }

        camera.gameObject.transform.position = new Vector3(xTarget, yTarget, camera.gameObject.transform.position.z);
    }
}
