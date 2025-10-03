using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Vector3 direction;
    private Camera cam;
    private Rigidbody rb;
    private float speed = 0.25f;
    private void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraForward = cam.transform.forward;
        rb.AddForce(cameraForward * speed, ForceMode.Acceleration);
        //transform.Translate(direction * moveSpeed * Time.deltaTime);
    }
}
