using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
	Vector2 velocity = new Vector2(1.0f, 0.0f);
	Vector2 acceleration = new Vector2(0.0f, 0.0f);
																		
	float rotationSpeed = 100.0f;
	float hoverHeight = 0.2f;
	float elasticityFactor = -0.5f;
	LayerMask trackLayer;

	Transform previousValidTransform;

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
            previousValidTransform = transform;

			Vector3 newUp = hit.normal;
			Vector3 newForward = Vector3.Cross(transform.right, hit.normal).normalized;
			Vector3 newRight = Vector3.Cross(hit.normal, transform.forward).normalized;

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, newRight);
            matrix.SetColumn(1, newUp);
            matrix.SetColumn(2, newForward);
            matrix[3, 3] = 1.0f;
            transform.rotation = Quaternion.Slerp(transform.rotation, matrix.rotation, Time.deltaTime * rotationSpeed);

            Debug.DrawRay(transform.position, transform.TransformDirection(newForward),Color.blue);
			Debug.DrawRay(transform.position, transform.TransformDirection(newRight), Color.red);
            Debug.DrawRay(transform.position, transform.TransformDirection(newUp), Color.green);

            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            //float displacement = Vector3.Distance(hit.point, transform.position);
            //acceleration.y = elasticityFactor * displacement;

            //if (displacement > hoverHeight)
            //    velocity.y += acceleration.y * Time.deltaTime;
            //else
            //    velocity.y = 0.0f;

            transform.localPosition += transform.TransformDirection(new Vector3(moveHorizontal * velocity.x * Time.deltaTime, velocity.y * Time.deltaTime, moveVertical * velocity.x * Time.deltaTime));
        }
		else 
		{
			Debug.Log("No hit");
		}
		
	}
}
