using UnityEngine;

public class TestingJoystick : MonoBehaviour
{
    public Joystick joystick;
    public Transform turret;

    void Update()
    {
        float x = joystick.Horizontal;
        float y = joystick.Vertical;

        // Check if the joystick is actually being moved
        if (x != 0.0f || y != 0.0f)
        {
            float angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            Debug.Log(angle);
            turret.rotation = Quaternion.Euler(0, -(angle - 90), 0);
            // 'angle' now holds the direction in degrees (0 to 360 or -180 to 180)
        }
    }
}
