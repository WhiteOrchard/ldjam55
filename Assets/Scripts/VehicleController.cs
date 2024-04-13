using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
	public Transform forwardAnchor;
	public Transform backAnchorL;
	public Transform backAnchorR;

	public float anchorFowardDistance = 0.05f;
	public float anchorSideDistance = 0.05f;
	public float anchorUpDistance = 0.05f;
	public float anchorBackwardDistance = 0.05f;

	float rotationSpeed = 100f;

	Vector2 velocity = new Vector2(1.0f, 0.0f);
    float trackAdjustmentSpeed = 100.0f;

    LayerMask trackLayer;

	void Start()
	{
		trackLayer = 1 << LayerMask.NameToLayer("RaceTrack");
	}

	void Update()
	{
		RaycastHit hit;
		bool hasHit = Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.up), out hit, 10.0f, trackLayer);

		if (hasHit)
		{
            Vector3 newUp = hit.normal.normalized;
			Vector3 newForward = Vector3.Cross(transform.right, hit.normal).normalized;
			Vector3 newRight = Vector3.Cross(hit.normal, transform.forward).normalized;

			Debug.DrawRay(hit.point, newUp, Color.yellow);
            Debug.DrawRay(transform.position, newForward, Color.blue);
            Debug.DrawRay(transform.position, newRight, Color.red);
            Debug.DrawRay(transform.position, newUp, Color.green);

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, newRight);
            matrix.SetColumn(1, newUp);
            matrix.SetColumn(2, newForward);
            matrix[3, 3] = 1.0f;
            transform.rotation = matrix.rotation;
			transform.Rotate(transform.up, moveHorizontal * rotationSpeed * Time.deltaTime, Space.World);

            transform.localPosition += transform.TransformDirection(new Vector3(0, velocity.y * Time.deltaTime, moveVertical * velocity.x * Time.deltaTime));

			forwardAnchor.position = transform.position + anchorFowardDistance * transform.forward;
			backAnchorL.position = transform.position 
				- anchorSideDistance * transform.right
				- anchorBackwardDistance * transform.forward 
				+ anchorUpDistance * transform.up;
            backAnchorR.position = transform.position
                + anchorSideDistance * transform.right
                - anchorBackwardDistance * transform.forward
                + anchorUpDistance * transform.up;
        }
		else 
		{
			Debug.Log("No hit");
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
}
