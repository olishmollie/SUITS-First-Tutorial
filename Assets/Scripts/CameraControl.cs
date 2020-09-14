using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography.X509Certificates;
//using System.Diagnostics;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float mouseSensitivity = 1;
    public float cameraSpeed = 10;
    public float clampAngle = 80;

    public GameObject ballPrefab;

    // FixedUpdate for the move
    void Update()
    {
        // Grab inputs first
        // Keyboard 
        Vector3 translation = new Vector3(0, 0, 0);

        translation.z = Input.GetAxisRaw("Vertical");
        translation.x = Input.GetAxisRaw("Horizontal");
        translation.y = Input.GetAxisRaw("Jump");

        // Mouse 
        float horz = 0;
        float vert = 0;


        horz = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        vert = -Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        //  Relative translation because it depends on the normal
        translation *= Time.deltaTime * cameraSpeed;
        transform.Translate(translation);

        // Rotation
        if (Cursor.lockState != CursorLockMode.None)
        {
            transform.RotateAround(transform.position, Vector3.up, horz);
            Vector3 rot = transform.localRotation.eulerAngles;
            rot.x = (transform.localRotation.eulerAngles.x + 180) % 360 - 180 + vert;
            rot.x = Mathf.Clamp(rot.x, -clampAngle, clampAngle);
            transform.localRotation = Quaternion.Euler(rot);
        }


        // Mouse control
        if (Input.GetMouseButton(1)) // Right click
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

        // Mouse control
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            // Add stuff here
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);

            if (hitSomething)
            {
                GameObject ball = Instantiate(ballPrefab, hit.point, Quaternion.identity) as GameObject;

                Collider collider = ball.GetComponent<Collider>();

                // Raise the ball above the floor.
                ball.transform.localPosition = new Vector3(hit.point.x, hit.point.y + collider.bounds.extents.y, hit.point.z);

                // Turn the ball so that its normal vector is perpendicular to the floor.
                ball.transform.localRotation = Quaternion.FromToRotation(ball.transform.localPosition, hit.normal);
            }

        }
    }
}
