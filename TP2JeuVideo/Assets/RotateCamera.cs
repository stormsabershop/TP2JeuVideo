using UnityEngine;

public class RotateCamera : MonoBehaviour
{
    public float rotationSpeed = 90f; // deg/s

    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // touches/gauche droite
        if (Mathf.Abs(h) > 0.01f)
        {
            transform.Rotate(0f, h * -rotationSpeed * Time.deltaTime, 0f);
        }
    }
}
