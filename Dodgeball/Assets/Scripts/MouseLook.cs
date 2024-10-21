using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Outside Refrences")]
    public Transform playerBody;

    [Header("Camera Controls")]
    public float mouseXSensitivity = 100;
    public float mouseYSensitivity = 100;


    private float xRotation = 0;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        PlayerRoatation();
    }

    public void PlayerRoatation()
    {

        float mouseX = Input.GetAxis("Mouse X") * mouseXSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseYSensitivity * Time.deltaTime;

        //float controllerX = Input.GetAxis("Controller X") * mouseSensitivity * Time.deltaTime;
        //float controllerY = Input.GetAxis("Controller Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90, 90);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}