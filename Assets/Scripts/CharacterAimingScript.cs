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

    public GameObject weaponObject;

    

    Camera mainCamera;
    RaycastWeapon weapon;

    void Start()
    {
        mainCamera = Camera.main;
        weapon = weaponObject.GetComponent<RaycastWeapon>();
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

        if (PlayerController.shooterMode)
        {
            if (Input.GetButton("Fire1"))
            {
                weapon.Shoot();
            }
            
            //if (weapon.isFiring)
            //{
            //    weapon.UpdateFiring(Time.deltaTime);
            //}
            //weapon.UpdateBullets(Time.deltaTime);
            //if (Input.GetButtonUp("Fire1"))
            //{
            //    weapon.StopFiring();
            //}
        }
        
    }
}
