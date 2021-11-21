using UnityEngine;
 
public class FlyCamera : MonoBehaviour {
     
    float camSens = 0.05f;
    private Vector3 lastMouse = new Vector3(255, 255, 255);
     
    void Update () {
        lastMouse = Input.mousePosition - lastMouse ;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0 );
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x , transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;
        lastMouse =  Input.mousePosition;
    }
     
}