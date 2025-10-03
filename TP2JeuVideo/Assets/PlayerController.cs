using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Camera cam;
    private Rigidbody rb;
    public static GameObject player;
    public Renderer rend;
    private Coroutine powerUpCoroutine;
    private Vector3 originalScale;
    private PhysicsMaterial originalPhysMat;

    void Awake()
    {
        player = gameObject;
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalScale = transform.localScale;
        Collider col = GetComponent<Collider>();
        if (col) originalPhysMat = col.sharedMaterial;// sets the static reference
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (LevelController.instance != null && LevelController.instance.isGameOver) return;
        Vector3 cameraForward = cam.transform.forward;
        rb.AddForce(cameraForward * 2f, ForceMode.Acceleration);
        //transform.Translate(direction * moveSpeed * Time.deltaTime);
    }

    public void HitEffect()
    {
        StartCoroutine(FlashRed());
    }

    private IEnumerator FlashRed()
    {
        if (rend == null) yield break;
        Color orig = rend.material.color;
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        rend.material.color = orig;
    }
}
