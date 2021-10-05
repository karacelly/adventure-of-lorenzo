using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CharacterAimingScript : MonoBehaviour
{
    public float turnSpeed = 15;
    public float aimDuration = 0.3f;
    public Transform cameraLookAt;
    public Rig aimLayer;

    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    Camera mainCamera;
    RaycastWeapon weapon;

    void Start()
    {
        mainCamera = Camera.main;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        weapon = GetComponentInChildren<RaycastWeapon>();
    }

    void fixedUpdate()
    {
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);

        cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.deltaTime);


    }

    private void LateUpdate()
    {
        if (aimLayer)
        {
            if (Input.GetButton("Fire2"))
            {
                aimLayer.weight += Time.deltaTime / aimDuration;
            }
            else
            {
                aimLayer.weight -= Time.deltaTime / aimDuration;
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            weapon.StartFiring();
        }
        if (Input.GetButtonUp("Fire1"))
        {
            weapon.StopFiring();
        }
    }
}
