using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateScript : MonoBehaviour
{
    public float speed = 10f;
    public float time = 0f;
    public float timeToChange = 3f;
    int state = 0;
    public Animator animator;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime);
        if (time < timeToChange)
        {
            time += Time.deltaTime;
        }
        else
        {
            time = 0f;
            switch (state)
            {
                case 0:
                    animator.SetBool("isMoving", true);
                    state = 1;
                    break;
                case 1:
                    animator.SetBool("isTurbo", true);
                    state = 2;
                    break;
                case 2:
                    animator.SetBool("isTurbo", false);
                    state = 3;
                    break;
                case 3:
                    animator.SetBool("isMoving", false);
                    state = 0;
                    break;
            }
        }
        
    }
}
