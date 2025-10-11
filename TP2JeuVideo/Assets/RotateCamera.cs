using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed = 100f; // deg/s
    public bool autoRotate = false; // Add this line

    void Update()
    {
        // If autoRotate is true, rotate the camera automatically
        if (autoRotate)
        {
            // The negative sign makes it rotate right. You can remove it to rotate left.
            transform.Rotate(0f, -rotationSpeed * Time.deltaTime, 0f);
        }
        else // Otherwise, use player input
        {
            float h = Input.GetAxis("Horizontal"); // touches/gauche droite
            if (Mathf.Abs(h) > 0.01f)
            {
                transform.Rotate(0f, h * -rotationSpeed * Time.deltaTime, 0f);
            }
        }
    }
}