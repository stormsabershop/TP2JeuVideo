using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody rb;
    public Renderer rend;
    //private GameObject self; // inutile
    private float baseForce = 0.2f;
    private float difficulty = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = GetComponentInChildren<Rigidbody>();

        rend = GetComponent<Renderer>();
        if (rend == null) rend = GetComponentInChildren<Renderer>();

        if (rb == null) Debug.LogError("EnemyController: Rigidbody missing on enemy prefab!");
        if (rend == null) Debug.LogWarning("EnemyController: Renderer missing on enemy prefab.");
    }

    public void InitializeEnemy(float size = 1f)
    {
        transform.localScale = Vector3.one * size;

        if (rb != null)
        {
            rb.mass = Mathf.Clamp(size * 1.0f, 0.1f, 20f);
        }
    }


    void FixedUpdate()
    {
        if (LevelController.instance != null && LevelController.instance.gameOver) return;
        if (PlayerController.player == null) return;
        if (rb == null) return;

        
        float playerScale = 1f;
        if (PlayerController.player != null)
            playerScale = PlayerController.player.transform.localScale.x;

        // Exemple : si le joueur est 2x plus gros, l'ennemi devient 2x plus léger
        // Clamp pour éviter des masses négatives ou ridicules
        rb.mass = Mathf.Clamp(2f / playerScale, 0.2f, 2f);
        // --- fin ajustement ---

        Vector3 dir = PlayerController.player.transform.position - transform.position;
        dir.y = 0f;
        dir.Normalize();
        rb.AddForce(dir * baseForce * difficulty, ForceMode.Force);

        if (transform.position.y < -5f)
        {
            if (LevelController.instance != null)
                LevelController.instance.EnemyOutOfBound();

            Destroy(gameObject);
        }
    }
}
