using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
	public Transform forwardAnchor;
	public Transform backAnchorL;
	public Transform backAnchorR;

    public Animator animator;
    private bool isMoving = false;
    private bool isTurbo = false;

    public float anchorFowardDistance = 0.05f;
	public float anchorSideDistance = 0.05f;
	public float anchorUpDistance = 0.05f;
	public float anchorBackwardDistance = 0.05f;

	float rotationSpeed = 100f;
	float trackAdjustmentSpeed = 10f;
	float cameraAdjustmentTime = 0.03f;
    Vector3 velocityForward = Vector3.zero;
    Vector3 velocityBackL = Vector3.zero;
    Vector3 velocityBackR = Vector3.zero;

    float springConstant = 100f;
    float hoveringBaseHeight = 0.1f;

	Vector2 velocity = new Vector2(5.0f, 0.0f);

    LayerMask trackLayer;

    float turboDuration = 1.0f;

    int lastCrossedSector = -1;
    List<float> lapTimes = new List<float>();
    float totalTime = 0;

	void Start()
	{
		trackLayer = 1 << LayerMask.NameToLayer("RaceTrack");
	}

	void Update()
	{
		RaycastHit hit;
		bool hasHit = Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, 10.0f, trackLayer);

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (hasHit)
        {
            Vector3 targetUp = hit.normal.normalized;
            Vector3 targetForward = Vector3.Cross(transform.right, hit.normal).normalized;
            Vector3 targetRight = Vector3.Cross(hit.normal, transform.forward).normalized;

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, targetRight);
            matrix.SetColumn(1, targetUp);
            matrix.SetColumn(2, targetForward);
            matrix[3, 3] = 1.0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, matrix.rotation, trackAdjustmentSpeed * Time.deltaTime);
            transform.Rotate(transform.up, moveHorizontal * rotationSpeed * Time.deltaTime, Space.World);

            Debug.DrawRay(hit.point, targetUp, Color.yellow);
            Debug.DrawRay(transform.position, targetForward, Color.blue);
            Debug.DrawRay(transform.position, targetRight, Color.red);
            Debug.DrawRay(transform.position, targetUp, Color.green);

            float distanceToGround = hit.distance;
            float springForce = springConstant * (hoveringBaseHeight - distanceToGround);
            velocity.y = springForce;
        }
        else
        {
            float springForce = springConstant * hoveringBaseHeight;
            velocity.y += springForce;
        }

        transform.localPosition += transform.TransformDirection(new Vector3(0, velocity.y * Time.deltaTime, moveVertical * velocity.x * Time.deltaTime));

        Vector3 targetForwardAnchorPosition = transform.position + anchorFowardDistance * transform.forward;
        Vector3 targetBackAnchorLPosition = transform.position
            - anchorSideDistance * transform.right
            - anchorBackwardDistance * transform.forward
            + anchorUpDistance * transform.up;
        Vector3 targetBackAnchorRPosition = transform.position
            + anchorSideDistance * transform.right
            - anchorBackwardDistance * transform.forward
            + anchorUpDistance * transform.up;

        forwardAnchor.position = Vector3.SmoothDamp(forwardAnchor.position, targetForwardAnchorPosition, ref velocityForward, cameraAdjustmentTime);
        backAnchorL.position = Vector3.SmoothDamp(backAnchorL.position, targetBackAnchorLPosition, ref velocityBackL, cameraAdjustmentTime);
        backAnchorR.position = Vector3.SmoothDamp(backAnchorR.position, targetBackAnchorRPosition, ref velocityBackR, cameraAdjustmentTime);

        isMoving = (moveHorizontal != 0.0f || moveVertical != 0.0f);
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isTurbo", isTurbo);

        if (isTurbo)
        {
            turboDuration -= Time.deltaTime;
            Debug.Log(turboDuration);
        }

        if (turboDuration < 0)
        {
            turboDuration = 1;
            isTurbo = false;
            velocity.x /= 2.0f;
        }

        if (lapTimes.Count > 0)
        {
            lapTimes[lapTimes.Count-1] += Time.deltaTime;
            totalTime += Time.deltaTime;
        }
    }

	public Transform getForwardAnchor()
	{
		return forwardAnchor;
	}

	public Transform getBackAnchorL()
	{
        return backAnchorL;
    }

	public Transform getBackAnchorR() 
	{
		return backAnchorR;
	}

	public Vector3 getAnchorNormal()
	{
        Vector3 v1 = forwardAnchor.position - backAnchorL.position;
		Vector3 v2 = forwardAnchor.position - backAnchorR.position;
		return Vector3.Cross(v2, v1).normalized;
    }

	public Vector3 getAnchorRight()
	{
        return (backAnchorR.position - backAnchorL.position).normalized;
    }

	public Vector3 getAnchorForward()
	{
        return (forwardAnchor.position - (backAnchorR.position + backAnchorL.position) / 2.0f).normalized;
    }

    public Vector3 getCameraPos()
    {
        return (backAnchorR.position + backAnchorL.position) / 2.0f;
    }

    public void enableTurbo()
    {
        if (!isTurbo)
        {
            isTurbo = true;
            velocity.x *= 2.0f;
        }
    }
    public void setLastCrossedSector(int sectorNumber)
    {
        if (sectorNumber <= lastCrossedSector)
            lastCrossedSector = sectorNumber;
    }

    public void newLap()
    {
        lapTimes.Add(0);
    }

    public List<float> getLapTimes()
    {
        return lapTimes;
    }

    public float getCurrentLapTime()
    {
        if (lapTimes.Count == 0)
            return 0;
        return lapTimes.Last();
    }

    public float getTotalTime()
    {
        return totalTime;
    }
}
