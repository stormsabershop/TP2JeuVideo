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
    private float moveForce = 4f;
    private bool isBouncyActive = false;
    private int bouncyCharges = 0;
    private Collider resetCol;

    public float hitSomething = 0f;
    public float dureeHit = 0.01f;

    private MaterialPropertyBlock propBlock;
    private Material matInstance;

    private Coroutine blinkCoroutine;


    void Awake()
    {
        player = gameObject;
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        originalScale = transform.localScale;
        Collider col = GetComponent<Collider>();
        matInstance = rend.material;
        propBlock = new MaterialPropertyBlock();
        if (col) originalPhysMat = col.sharedMaterial;
    }

   
    void FixedUpdate()
    {
        if (LevelController.instance != null && LevelController.instance.gameOver) return;
        Vector3 cameraForward = cam.transform.forward;
        float vInput = Input.GetAxis("Vertical"); 
        if (Mathf.Abs(vInput) > 0.01f)
        {
            Vector3 camForward = cam.transform.forward;
            camForward.y = 0f; 
            camForward.Normalize();
            rb.AddForce(camForward * vInput * moveForce, ForceMode.Force);
        }
        



        if (transform.position.y < -5f && LevelController.instance != null && !LevelController.instance.gameOver)
        {
            LevelController.instance.GameOver();
        }

    }


    public void EnablePowerUp(PowerUp.PowerUpType type, float duration = 8f)
    {
        if (powerUpCoroutine != null)
            StopCoroutine(powerUpCoroutine);
        powerUpCoroutine = StartCoroutine(HandlePowerUp(type, duration));
    }

    private IEnumerator HandlePowerUp(PowerUp.PowerUpType type, float duration)
    {
        switch (type)
        {
            case PowerUp.PowerUpType.IncreaseSize:
                transform.localScale = originalScale * 1.5f;
                rb.mass *= 2f;
                
                break;

            case PowerUp.PowerUpType.BouncyPhysic:
                isBouncyActive = true;
                bouncyCharges = 3;

                Collider col = GetComponent<Collider>();

            

                break;

        }

        if (type != PowerUp.PowerUpType.BouncyPhysic)
        {
            // si ton shader a une propriété "_GlowAmount"
            if (rend.material.HasProperty("_GlowAmount"))
            {
                rend.material.SetFloat("_GlowAmount", 0.4f);
            }

            yield return new WaitForSeconds(duration);

            if (rend.material.HasProperty("_GlowAmount"))
            {
                rend.material.SetFloat("_GlowAmount", 0f);
            }

            // Reset stats
            transform.localScale = originalScale;
            rb.mass = 1f;
            resetCol = GetComponent<Collider>();
           
            if (resetCol != null && originalPhysMat != null)
                resetCol.material = originalPhysMat;
        }
        else
        {
            if (rend.material.HasProperty("_GlowAmount"))
            {
                rend.material.SetFloat("_GlowAmount", 0.4f);
            }
        }
   
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkEffect());
            Rigidbody enemyRb = collision.rigidbody;

            if (enemyRb != null)
            {
                Vector3 dir = (collision.transform.position - transform.position).normalized;
                dir.y = 0.3f; 

                if (isBouncyActive)
                {
                   
                    float launchForce = 40f;
                    enemyRb.AddForce(dir * launchForce, ForceMode.Impulse);

                    bouncyCharges--;
                    Debug.Log($" Bouncy hit! Remaining charges: {bouncyCharges}");

                    if (bouncyCharges <= 0)
                        EndBouncyPowerUp();
                }
                else
                {
                    
                    float normalForce = 4f;
                    enemyRb.AddForce(dir * normalForce, ForceMode.Impulse);
                }
            }
        }
    }


    private IEnumerator BlinkEffect()
    {
        float timer = dureeHit; // on part de la durée totale

        while (timer > 0)
        {
            float blinkAmount = timer / dureeHit; // 1  0 sur la durée exacte
            SetBlink(blinkAmount);

            timer -= Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(dureeHit);
        SetBlink(0f); // assure que le blink est off
    }


    private void SetBlink(float amount)
    {
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Blink_Amount", amount);
        rend.SetPropertyBlock(propBlock);
    }

    private void AnnuleFlash()
    {
        matInstance.SetFloat("_Hit", 0f);
    }
    private void EndBouncyPowerUp()
    {
        isBouncyActive = false;
        bouncyCharges = 0;

        Collider col = GetComponent<Collider>();
        if (col != null && originalPhysMat != null)
            col.material = originalPhysMat;

        if (rend != null)
            rend.material.color = Color.white;
        if (rend.material.HasProperty("_GlowAmount"))
        {
            rend.material.SetFloat("_GlowAmount", 0f);
        }

        Debug.Log("Bouncy Power Up expired!");
    }


}
