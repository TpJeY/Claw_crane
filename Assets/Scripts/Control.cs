using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Control : MonoBehaviour
{
    public Transform Joystick;
    public Transform Button;
    public Transform Arm;
    public Animator animator;
    public Text text;

    movementParameters JoystickParametersX;
    movementParameters JoystickParametersY;
    movementParameters buttonParameters;
    movementParameters lowerMechanismParameters;
    movementParameters armParametersAxisX;
    movementParameters armParametersAxisZ;
    Transform lowerMechanism;
    Vector3 defaultJoysticPosition;
    float deviationAngleX;
    float deviationAngleY;
    float openClawTime;
    float closeClawTime;
    float basketSize;
    bool clawCanMove;
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
        deviationAngleX = 10f;
        deviationAngleY = 10f;
        JoystickParametersX = new movementParameters(defaultJoysticPosition.z + deviationAngleX, defaultJoysticPosition.z - deviationAngleX, 0.5f);
        JoystickParametersY = new movementParameters(defaultJoysticPosition.x + deviationAngleY, defaultJoysticPosition.x - deviationAngleY, 0.5f);

        buttonParameters = new movementParameters(0, -0.025f, 0.005f);

        lowerMechanismParameters = new movementParameters(6, 2, 0.01f);

        armParametersAxisX = new movementParameters(0.5f, -0.5f, 0.002f);
        armParametersAxisZ = new movementParameters(0.25f, -0.75f, 0.002f);
        basketSize = 0.4f;

        clawCanMove = true;

        openClawTime = 1f;
        closeClawTime = 1f;

        StartCoroutine(CloseClaw(closeClawTime));
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
                if (clawCanMove && (Arm.localPosition.x < armParametersAxisX.MaxValue-basketSize || Arm.localPosition.z < armParametersAxisZ.MaxValue - basketSize))
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
                StartCoroutine(RotateObjectAxisX(Joystick, 1, JoystickParametersX));
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
                StartCoroutine(RotateObjectAxisX(Joystick, -1, JoystickParametersX));
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
            if (Mathf.Abs(Joystick.transform.localEulerAngles.z - defaultJoysticPosition.z) > JoystickParametersX.Step)
                StartCoroutine(RotateObjectInDefaultPositionAxisX(Joystick, defaultJoysticPosition, JoystickParametersX));
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            try
            {
                StartCoroutine(RotateObjectAxisY(Joystick, -1, JoystickParametersY));
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
                StartCoroutine(RotateObjectAxisY(Joystick, 1, JoystickParametersY));
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
            if (Mathf.Abs(Joystick.transform.localEulerAngles.x - defaultJoysticPosition.x) > JoystickParametersY.Step)
                StartCoroutine(RotateObjectInDefaultPositionAxisY(Joystick, defaultJoysticPosition, JoystickParametersY));
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            try
            {
                Application.Quit();
            }
            catch (Exception e)
            {
                print(e.Message);
            }
        }
        if (Input.anyKeyDown)
            text.text = "";

    }

    IEnumerator RotateObjectAxisX(Transform movable_object, float direction, movementParameters parameters)
    {
        if (direction > 0 && movable_object.transform.localEulerAngles.z < parameters.MaxValue || direction < 0 && movable_object.transform.localEulerAngles.z > parameters.MinValue)
        {
            movable_object.transform.localEulerAngles += new Vector3(0, 0, parameters.Step * (direction > 0 ? 1 : -1));
        }
        yield return null;
    }

    IEnumerator RotateObjectInDefaultPositionAxisX(Transform movable_object, Vector3 default_position, movementParameters parameters)
    {
        float step = movable_object.transform.localEulerAngles.z < default_position.z ? parameters.Step : -parameters.Step;
        movable_object.transform.localEulerAngles += new Vector3(0, 0, step);
        yield return null;
    }

    IEnumerator RotateObjectAxisY(Transform movable_object, float direction, movementParameters parameters)
    {
        if (direction > 0 && movable_object.transform.localEulerAngles.x < parameters.MaxValue || direction < 0 && movable_object.transform.localEulerAngles.x > parameters.MinValue)
        {
            movable_object.transform.localEulerAngles += new Vector3(parameters.Step * (direction > 0 ? 1 : -1), 0, 0);
        }
        yield return null;
    }

    IEnumerator RotateObjectInDefaultPositionAxisY(Transform movable_object, Vector3 default_position, movementParameters parameters)
    {
        float step = movable_object.transform.localEulerAngles.x < default_position.x ? parameters.Step : -parameters.Step;
        movable_object.transform.localEulerAngles += new Vector3(step, 0, 0);
        yield return null;
    }

    IEnumerator LowerArm()
    {
        clawCanMove = false;
        float updateTime = 0.02f;
        yield return OpenClaw(openClawTime);
        while (lowerMechanism.localPosition.y>lowerMechanismParameters.MinValue)
        {
            StartCoroutine(MoveObjectAxisY(lowerMechanism, -1, lowerMechanismParameters));
            yield return new WaitForSeconds(updateTime);
        }
        yield return CloseClaw(closeClawTime);
        while (lowerMechanism.localPosition.y < lowerMechanismParameters.MaxValue)
        {
            StartCoroutine(MoveObjectAxisY(lowerMechanism, 1, lowerMechanismParameters));
            yield return new WaitForSeconds(updateTime);
        }
        while (Arm.localPosition.x < armParametersAxisX.MaxValue- basketSize/5 || Arm.localPosition.z < armParametersAxisZ.MaxValue - basketSize/5)
        {
            StartCoroutine(MoveObjectAxisX(Arm, 1, armParametersAxisX));
            StartCoroutine(MoveObjectAxisZ(Arm, 1, armParametersAxisZ));
            yield return new WaitForSeconds(updateTime);
        }
        yield return new WaitForSeconds(1);
        yield return OpenClaw(openClawTime);
        yield return new WaitForSeconds(1);
        yield return CloseClaw(closeClawTime);
        yield return new WaitForSeconds(1);
        clawCanMove = true;
        yield return null;
    }

    IEnumerator OpenClaw(float time)
    {
        animator.SetBool("Open", true);
        animator.SetBool("Close", false);
        yield return new WaitForSeconds(time);
        yield return null;
    }

    IEnumerator CloseClaw(float time)
    {
        animator.SetBool("Open", false);
        animator.SetBool("Close", true);
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
