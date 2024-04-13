using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
	public Transform forwardAnchor;
	public Transform backAnchorL;
	public Transform backAnchorR;

    public Animator animator;
    private bool isMoving;
    private bool isTurbo;

    public float anchorFowardDistance = 0.05f;
	public float anchorSideDistance = 0.05f;
	public float anchorUpDistance = 0.05f;
	public float anchorBackwardDistance = 0.05f;

	float rotationSpeed = 100f;
	float trackAdjustmentSpeed = 10f;
	float cameraAdjustmentSpeed = 20f;

    float springConstant = 100f;
    float hoveringBaseHeight = 0.1f;

	Vector2 velocity = new Vector2(5.0f, 0.0f);

    LayerMask trackLayer;

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

        forwardAnchor.position = Vector3.Lerp(forwardAnchor.position, targetForwardAnchorPosition, cameraAdjustmentSpeed * Time.deltaTime);
        backAnchorL.position = Vector3.Lerp(backAnchorL.position, targetBackAnchorLPosition, cameraAdjustmentSpeed * Time.deltaTime);
        backAnchorR.position = Vector3.Lerp(backAnchorR.position, targetBackAnchorRPosition, cameraAdjustmentSpeed * Time.deltaTime);

        isMoving = (moveHorizontal != 0.0f || moveVertical != 0.0f);
		isTurbo = false;
        animator.SetBool("isMoving", isMoving);
        animator.SetBool("isTurbo", isTurbo);
        //if (isTurbo && !Input.GetKey(KeyCode.LeftShift))
        //{
        //    float currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        //		animator.Play("Move", 0, currentTime);
        //}

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
}
