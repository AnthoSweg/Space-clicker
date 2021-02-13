using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    Transform cam;
    Transform parent;

    Vector3 localRot;
    float camDistance = 10f;

    public float mouseSensitivity = 4f;
    public float scrollSensitvity = 2f;
    public float orbitDampening = 10f;
    public float scrollDampening = 6f;
    public Vector2 zoomMinMax;
    public float yOffset;

    public bool CameraDisabled = false;


    // Use this for initialization
    void Start()
    {
        this.cam = this.transform;
        this.parent = this.transform.parent;
    }


    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            CameraDisabled = !CameraDisabled;

        if (!CameraDisabled)
        {
            if (Input.GetMouseButton(0))
            {
                //Rotation of the Camera based on Mouse Coordinates
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    localRot.x += Input.GetAxis("Mouse X") * mouseSensitivity;
                    localRot.y -= Input.GetAxis("Mouse Y") * mouseSensitivity;

                    ////Clamp the y Rotation to horizon and not flipping over at the top
                    //if (_LocalRotation.y < -90f)
                    //    _LocalRotation.y = -90f;
                    //else if (_LocalRotation.y > 90f)
                    //    _LocalRotation.y = 90f;
                }
            }
            //Zooming Input from our Mouse Scroll Wheel
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensitvity;

                ScrollAmount *= (this.camDistance * 0.3f);

                this.camDistance += ScrollAmount * -1f;

                this.camDistance = Mathf.Clamp(this.camDistance, zoomMinMax.x, zoomMinMax.y);
                
               // this.cam.localPosition = new Vector3(0f, newY, Mathf.Lerp(this.cam.localPosition.z, this.camDistance * -1f, Time.deltaTime * ScrollDampening));

            }
        }

        //Actual Camera Rig Transformations
        Quaternion q = Quaternion.Euler(localRot.y, localRot.x, 0);
        this.parent.rotation = Quaternion.Lerp(this.parent.rotation, q, Time.deltaTime * orbitDampening);

        if (this.cam.localPosition.z != this.camDistance * -1f)
        {
            float newY = /*yOffset +*/ ExtensionMethods.Remap(camDistance, zoomMinMax, new Vector2(0, yOffset));
            this.cam.localPosition = Vector3.Lerp(this.cam.localPosition, new Vector3(0f, newY, this.camDistance * -1f), Time.deltaTime*scrollDampening);
//            this.cam.localPosition = new Vector3(0f, newY, Mathf.Lerp(this.cam.localPosition.z, this.camDistance * -1f, Time.deltaTime * scrollDampening));
            // Debug.Log("fixed it");
        }
    }
}
