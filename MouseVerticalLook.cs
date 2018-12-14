using UnityEngine;
using System.Collections;

/// MouseLook rotates the transform based on the mouse delta.
/// Minimum and Maximum values can be used to constrain the possible rotation

/// To make an FPS style character:
/// - Create a capsule.
/// - Add the MouseLook script to the capsule.
///   -> Set the mouse look to use LookX. (You want to only turn character but not tilt it)
/// - Add FPSInputController script to the capsule
///   -> A CharacterMotor and a CharacterController component will be automatically added.

/// - Create a camera. Make the camera a child of the capsule. Reset it's transform.
/// - Add a MouseLook script to the camera.
///   -> Set the mouse look to use LookY. (You want the camera to tilt up and down like a head. The character already turns.)
[AddComponentMenu("Camera-Control/Mouse Look")]
public class MouseVerticalLook : MonoBehaviour {

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -100F;
    public float maximumY = 100F;
    bool thirdPerson = false;
    bool fp = false;
    bool isGrabbing;
    bool isSliding;
    Player p1;
    float rotationY = 0F;
    float sensitivityXOld;
    public bool isActive = false;
    private void Start()
    {
       // float sensitivityXOld = sensitivityX;
    }
    void Update()
    {
        if (transform.tag == "Swat")
        {
            if (transform.GetComponent<PlayerAI>().selected)
                thirdPerson = false;
        }
        p1 = transform.root.GetComponent<Player>();
        if (Input.GetMouseButton(1) && isActive)
        {
            if (thirdPerson)
            {

                if (p1.isragdoll)
                {
                    sensitivityX = 0;
                }

                else
                {
                    sensitivityX = 2;
                }
                isGrabbing = p1.isGrabbing;
                isSliding = p1.isSliding;
            }
            if (!isGrabbing && !isSliding && axes == RotationAxes.MouseXAndY)
            {
                float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
            }
            else if (axes == RotationAxes.MouseX)
            {
                transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
            }
            else
            {
                rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
                rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

                transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
            }
        }
    }
}
	