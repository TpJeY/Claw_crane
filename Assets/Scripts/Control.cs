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
    Vector3 defaultJoysticPosition;
    movementParameters buttonParameters;
    movementParameters lowerMechanismParameters;
    float lowerMechanismTime;
    movementParameters armParametersAxisX;
    movementParameters armParametersAxisZ;
    bool clawCanMove;
    float openClawTime;
    float closeClawTime;

    struct movementParameters
    {
        float maxValue;
        float minValue;
        float step;

        public movementParameters(float maxValue, float minValue, float step)
        {
            this.maxValue = maxValue;
            this.minValue = minValue;
            this.step = step;
        }

        public float MaxValue { get => maxValue; set => maxValue = value; }
        public float MinValue { get => minValue; set => minValue = value; }
        public float Step { get => step; set => step = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        lowerMechanism = Arm.GetChild(0).GetChild(0);

        defaultJoysticPosition = Joystick.transform.eulerAngles;
        deviationStep = 0.5f;
        deviationAngle = 10f;

        buttonParameters = new movementParameters(0, -0.025f, 0.005f);

        lowerMechanismParameters = new movementParameters(6, 1, 0.01f);
        lowerMechanismTime = 7f;

        armParametersAxisX = new movementParameters(0.5f, -0.5f, 0.005f);
        armParametersAxisZ = new movementParameters(0.25f, -0.75f, 0.005f);

        clawCanMove = true;

        openClawTime = 1f;
        closeClawTime = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            try
            {
                StartCoroutine(MoveObjectAxisY(Button, -1, buttonParameters));
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
        else
            try
            {
                StartCoroutine(MoveObjectAxisY(Button, 1, buttonParameters));
            }
            catch (Exception e)
            {
                print(e.Message);
            }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            try
            {
                if (clawCanMove)
                    StartCoroutine(LowerArm(lowerMechanismTime));
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
                if (clawCanMove)
                    StartCoroutine(MoveObjectAxisX(Arm, -1, armParametersAxisX));
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
                if (clawCanMove)
                    StartCoroutine(MoveObjectAxisX(Arm, 1, armParametersAxisX));
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
                if (clawCanMove)
                    StartCoroutine(MoveObjectAxisZ(Arm, -1, armParametersAxisZ));
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
                if (clawCanMove)
                    StartCoroutine(MoveObjectAxisZ(Arm, 1, armParametersAxisZ));
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

    IEnumerator LowerArm(float time)
    {
        clawCanMove = false;
        float startFrameTime;
        float updateTime = 0.01f;
        float timePassed = 0;
        yield return OpenClaw(openClawTime);
        while (timePassed < time)
        {
            startFrameTime = Time.time;
            StartCoroutine(MoveObjectAxisY(lowerMechanism, -1, lowerMechanismParameters));
            yield return new WaitForSeconds(updateTime);
            timePassed += Time.time - startFrameTime;
        }
        yield return new WaitForSeconds(time);
        yield return CloseClaw(closeClawTime);
        timePassed = 0;
        while (timePassed < time)
        {
            startFrameTime = Time.time;
            StartCoroutine(MoveObjectAxisY(lowerMechanism, 1, lowerMechanismParameters));
            yield return new WaitForSeconds(updateTime);
            timePassed += Time.time - startFrameTime;
        }
        yield return OpenClaw(openClawTime);
        yield return CloseClaw(closeClawTime);
        clawCanMove = true;
        yield return null;
    }

    IEnumerator OpenClaw(float time)
    {
        yield return new WaitForSeconds(time);
        yield return null;
    }

    IEnumerator CloseClaw(float time)
    {
        yield return new WaitForSeconds(time);
        yield return null;
    }

    IEnumerator MoveObjectAxisX(Transform movable_object, float direction, movementParameters parameters)
    {
        if (direction > 0 && movable_object.transform.localPosition.x < parameters.MaxValue || direction < 0 && movable_object.transform.localPosition.x > parameters.MinValue)
        {
            movable_object.transform.localPosition += new Vector3(parameters.Step * (direction > 0 ? 1 : -1), 0, 0);
        }
        yield return null;
    }

    IEnumerator MoveObjectAxisY(Transform movable_object, float direction, movementParameters parameters)
    {
        if (direction > 0 && movable_object.transform.localPosition.y < parameters.MaxValue || direction < 0 && movable_object.transform.localPosition.y > parameters.MinValue)
        {
            movable_object.transform.localPosition += new Vector3(0, parameters.Step * (direction > 0 ? 1 : -1), 0);
        }
        yield return null;
    }

    IEnumerator MoveObjectAxisZ(Transform movable_object, float direction, movementParameters parameters)
    {
        if (direction > 0 && movable_object.transform.localPosition.z < parameters.MaxValue || direction < 0 && movable_object.transform.localPosition.z > parameters.MinValue)
        {
            movable_object.transform.localPosition += new Vector3(0, 0, parameters.Step * (direction > 0 ? 1 : -1));
        }
        yield return null;
    }
}
