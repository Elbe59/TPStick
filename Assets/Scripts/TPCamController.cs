using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPCamController : MonoBehaviour
{
    public float RotationSpeedX = 1;
    public float RotationSpeedY = 1;
    public Transform Target, Player, RagdollTarget;

    public Transform CamFocus;
    float mousex, mousey;
    float horizontal = 0f;
    float vertical = 0f;
    public bool deadChar;

    //Camera Wall Collision
    public float camDist = 7;
    public LayerMask colliderCamMask;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        deadChar = false;
        CamFocus = Target;
    }

    private void LateUpdate()
    {
        CamControl();
        SetCamDist();
    }

    void CamControl()
    {
        mousex += horizontal * RotationSpeedX;
        mousey += vertical * RotationSpeedY;
        mousey = Mathf.Clamp(mousey, -60, 60);


        Vector3 rotTarget = CamFocus.rotation.eulerAngles;
        transform.LookAt(CamFocus);
        CamFocus.rotation = Quaternion.Euler(mousey, rotTarget.y, 0);
        if (!deadChar)
        {
            Player.rotation = Quaternion.Euler(0, mousex, 0);
        }
    }

    void SetCamDist()
    {
        float camNewDist = camDist;
        Vector3 camRot = (transform.position - CamFocus.position).normalized;


        RaycastHit hit;
        if (Physics.Raycast(CamFocus.position, camRot, out hit, camNewDist, colliderCamMask))
        {
            float distCol = hit.distance;
            if (distCol < 2)
                distCol = 2;

            camNewDist = distCol;
        }

        transform.position = CamFocus.position + (camRot * camNewDist);
    }

    public void OnCameraH(InputValue value)
    {
        float zoneVal = value.Get<float>();
        horizontal = zoneVal / 3;
    }

    public void OnCameraCH(InputValue value)
    {
        float zoneVal = value.Get<float>();
        if (Mathf.Abs(zoneVal) <= 0.2f)
            zoneVal = 0;

        horizontal = zoneVal * 1.5f;
    }

    public void OnCameraCV(InputValue value)
    {
        float zoneVal = -value.Get<float>();
        if (Mathf.Abs(zoneVal) <= 0.2f)
            zoneVal = 0;

        vertical = zoneVal * 1.5f;
    }


    public void OnCameraV(InputValue value) {
        float zoneVal = -value.Get<float>();
        vertical = zoneVal / 3;
    }

}
