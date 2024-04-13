using UnityEngine;

public class VehicleController : MonoBehaviour
{
	public float speed = 10.0f;
	public float rotationSpeed = 10.0f;
	public float hoverHeight = 0.3f;
	private LayerMask trackLayer;

	void Start()
	{
		// Use the layer name to set the trackLayer for raycast filtering
		trackLayer = 1 << LayerMask.NameToLayer("RaceTrack");
	}

	void Update()
	{
		RaycastHit hit;
		Vector3 down = transform.TransformDirection(-Vector3.up);
		bool isGrounded = Physics.Raycast(transform.position, down, out hit, hoverHeight, trackLayer);

		// Visualize the raycast
		Debug.DrawRay(transform.position, down * hoverHeight, isGrounded ? Color.green : Color.red);

		if (isGrounded && hit.distance < hoverHeight)
		{
			// This calculates how far below the hoverHeight the vehicle is
			float heightError = hoverHeight - hit.distance;

		}


		// Rotate to align with the track's normal if we are grounded
		if (isGrounded)
		{
			Quaternion toRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, Time.deltaTime * rotationSpeed);
		}

		// Move forward based on input
		float moveHorizontal = Input.GetAxis("Horizontal");
		float moveVertical = Input.GetAxis("Vertical");

		// Translate the vehicle in the movement direction
		Vector3 moveDirection = new Vector3(moveHorizontal, 0.0f, moveVertical);
		moveDirection = transform.TransformDirection(moveDirection);
		transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
	}
}
