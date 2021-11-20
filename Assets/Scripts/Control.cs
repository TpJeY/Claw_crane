using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public Transform Joystick;
    public Transform Button;
    public Transform Arm;
    Transform lowerMechanism;
    float deviationAngle;
    float deviationStep;
    float buttonDistance;
    float buttonStep;
    float armDistance;
    float armLowerStep;
    float armStep;
    float armMinX;
    float armMinY;
    float armMaxX;
    float armMaxY;
    Vector3 defaultJoysticPosition;
    Vector3 defaultButtonPosition;
    Vector3 defaultArmPosition;
    // Start is called before the first frame update
    void Start()
    {
        lowerMechanism = Arm.GetChild(0).GetChild(0);
        defaultJoysticPosition = Joystick.transform.eulerAngles;
        defaultButtonPosition = Button.transform.localPosition;
        defaultArmPosition = lowerMechanism.transform.localPosition;
        deviationStep = 0.5f;
        deviationAngle = 10f;
        buttonStep = 0.005f;
        buttonDistance = 0.025f;
        armDistance = 1;
        armLowerStep = 0.005f;
        armStep = 0.005f;
        armMinX = -0.5f;
        armMinY = -0.75f;
        armMaxX = 0.5f;
        armMaxY = 0.25f;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            try
            {
                StartCoroutine(MoveButtonDown(-buttonStep));
                StartCoroutine(MoveArmDown(-armLowerStep));
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
        else
            try
            {
                if (Button.transform.localPosition.y < defaultButtonPosition.y)
                    StartCoroutine(MoveButtonUp(buttonStep));
                if (lowerMechanism.transform.localPosition.y < defaultArmPosition.y)
                    StartCoroutine(MoveArmUp(armLowerStep));
            }
            catch (Exception e)
            {
                print(e.Message);
            }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            try
            {
                StartCoroutine(LowerArm());
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            try
            {
                StartCoroutine(MoveJoystickAxisX(deviationStep));
                StartCoroutine(MoveArmAxisXPlus(armStep));
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            try
            {
                StartCoroutine(MoveJoystickAxisX(-deviationStep));
                StartCoroutine(MoveArmAxisXMinus(armStep));
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
        else
        {
            if (Mathf.Abs(Joystick.transform.localEulerAngles.z - defaultJoysticPosition.z) > deviationStep)
                StartCoroutine(MoveJoystickCenterAxisX());
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            try
            {
                StartCoroutine(MoveJoystickAxisY(-deviationStep));
                StartCoroutine(MoveArmAxisYPlus(armStep));
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            try
            {
                StartCoroutine(MoveJoystickAxisY(deviationStep));
                StartCoroutine(MoveArmAxisYMinus(armStep));
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
        else
        {
            if (Mathf.Abs(Joystick.transform.localEulerAngles.x - defaultJoysticPosition.x) > deviationStep)
                StartCoroutine(MoveJoystickCenterAxisY());
        }
    }
    IEnumerator MoveJoystickAxisX(float step)
    {
        if (Mathf.Abs(Joystick.transform.localEulerAngles.z - defaultJoysticPosition.z) < deviationAngle)
        {
            Joystick.transform.localEulerAngles += new Vector3(0, 0, step);
        }
        yield return null;
    }

    IEnumerator MoveJoystickCenterAxisX()
    {
        float step = Joystick.transform.localEulerAngles.z < defaultJoysticPosition.z ? deviationStep : -deviationStep;
        Joystick.transform.localEulerAngles += new Vector3(0, 0, step);
        yield return null;
    }

    IEnumerator MoveJoystickAxisY(float step)
    {
        if (Mathf.Abs(Joystick.transform.localEulerAngles.x - defaultJoysticPosition.x) < deviationAngle)
        {
            Joystick.transform.localEulerAngles += new Vector3(step, 0, 0);
        }
        yield return null;
    }

    IEnumerator MoveJoystickCenterAxisY()
    {
        float step = Joystick.transform.localEulerAngles.x < defaultJoysticPosition.x ? deviationStep : -deviationStep;
        Joystick.transform.localEulerAngles += new Vector3(step, 0, 0);
        yield return null;
    }

    IEnumerator MoveButtonDown(float step)
    {
        if (Button.transform.localPosition.y > -buttonDistance)
        {
            Button.transform.localPosition += new Vector3(0, step, 0);
        }
        yield return null;
    }

    IEnumerator MoveButtonUp(float step)
    {
        while (Button.transform.localPosition.y < defaultButtonPosition.y)
        {
            Button.transform.localPosition += new Vector3(0, step, 0);
        }
        yield return null;
    }

    IEnumerator LowerArm()
    {
        yield return null;
    }

    IEnumerator MoveArmAxisXPlus(float step)
    {
        if (Arm.transform.localPosition.x > armMinX)
            Arm.transform.localPosition -= new Vector3(step, 0, 0);
        yield return null;
    }
    IEnumerator MoveArmAxisYPlus(float step)
    {
        if (Arm.transform.localPosition.y > armMinY)
            Arm.transform.localPosition -= new Vector3(0, 0, step);
        yield return null;
    }

    IEnumerator MoveArmAxisXMinus(float step)
    {
        if (Arm.transform.localPosition.x < armMaxX)
            Arm.transform.localPosition += new Vector3(step, 0, 0);
        yield return null;
    }
    IEnumerator MoveArmAxisYMinus(float step)
    {
        if (Arm.transform.localPosition.z < armMaxY)
            Arm.transform.localPosition += new Vector3(0, 0, step);
        yield return null;
    }

    IEnumerator MoveArmDown(float step)
    {
        if (lowerMechanism.transform.localPosition.y > armDistance)
        {
            lowerMechanism.transform.localPosition += new Vector3(0, step, 0);
        }
        yield return null;
    }

    IEnumerator MoveArmUp(float step)
    {
        if (lowerMechanism.transform.localPosition.y < defaultArmPosition.y)
        {
            lowerMechanism.transform.localPosition += new Vector3(0, step, 0);
        }
        yield return null;
    }

    IEnumerator MoveObjectAxisX(Transform movable_object, float step, float minX, float maxX)
    {
        if (step>0 && movable_object.transform.localPosition.x < maxX || step < 0 && movable_object.transform.localPosition.x > minX)
        {
            movable_object.transform.localPosition += new Vector3(step, 0, 0);
        }
        yield return null;
    }

    IEnumerator MoveObjectAxisY(Transform movable_object, float step, float minY, float maxY)
    {
        if (step > 0 && movable_object.transform.localPosition.x < maxY || step < 0 && movable_object.transform.localPosition.x > minY)
        {
            movable_object.transform.localPosition += new Vector3(0, step, 0);
        }
        yield return null;
    }

    IEnumerator MoveObjectAxisZ(Transform movable_object, float step, float minZ, float maxZ)
    {
        if (step > 0 && movable_object.transform.localPosition.x < maxZ || step < 0 && movable_object.transform.localPosition.x > minZ)
        {
            movable_object.transform.localPosition += new Vector3(0, step, 0);
        }
        yield return null;
    }
}
