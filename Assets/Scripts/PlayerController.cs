using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerController : MonoBehaviour
{
    public Rig useWeapon;
    public static bool shooterMode;
    public GameObject weaponState;

    // Start is called before the first frame update
    void Start()
    {
        shooterMode = false;
        useWeapon.weight = 0;
        weaponState.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!UIController.inDialogue)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                shooterMode = !shooterMode;

                if (shooterMode)
                {
                    weaponState.SetActive(true);
                    StartCoroutine(SmoothRig(0f, 1f));
                }
                else
                {
                    weaponState.SetActive(false);
                    StartCoroutine(SmoothRig(1f, 0f));
                }
            }
        }
    }

    IEnumerator SmoothRig(float start, float end)

    {
        float elapsedTime = 0;

        float waitTime = 0.5f;

        while (elapsedTime < waitTime)
        {
            useWeapon.weight = Mathf.Lerp(start, end, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

    }
}
