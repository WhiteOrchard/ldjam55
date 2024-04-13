using UnityEngine;

public class VehicleAnimationController : MonoBehaviour
{
	private Animator animator;
	private bool isMoving;
	private bool isTurbo;

	void Start()
	{
		animator = GetComponent<Animator>();
	}

	void Update()
	{
		isMoving = Input.GetKey(KeyCode.Space);
		isTurbo = isMoving && Input.GetKey(KeyCode.LeftShift);

		// When the turbo key is released, smoothly transition back to Move at its current state.
		if (isTurbo && !Input.GetKey(KeyCode.LeftShift))
		{
			TransitionToMove();
		}

		animator.SetBool("isMoving", isMoving);
		animator.SetBool("isTurbo", isTurbo);
	}

	private void TransitionToMove()
	{
		// Continue the Move animation from where it left off when Turbo was initiated.
		// The 'normalizedTime' parameter allows you to start the animation at a given point in its timeline.
		// 0.0f is the start, 1.0f is the end. You will need to calculate the correct value based on your current state.

		float currentTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
		animator.Play("Move", 0, currentTime);
	}
}
