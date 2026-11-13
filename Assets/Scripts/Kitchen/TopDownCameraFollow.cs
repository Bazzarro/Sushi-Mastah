using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0f, 7.5f, -9.5f);
    public float smoothTime = 0.2f;

    private Vector3 velocity;

    private void Start()
    {
        SnapToTarget();
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 wantedPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, wantedPosition, ref velocity, smoothTime);
        LookAtTarget();
    }

    public void SnapToTarget()
    {
        if (target == null)
            return;

        transform.position = target.position + offset;
        velocity = Vector3.zero;
        LookAtTarget();
    }

    private void LookAtTarget()
    {
        Vector3 lookPoint = target.position + Vector3.up * 0.4f;
        transform.rotation = Quaternion.LookRotation(lookPoint - transform.position, Vector3.up);
    }
}
