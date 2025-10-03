using UnityEngine;

public class EnemyController : MonoBehaviour
{

    private Rigidbody rb;
    public Renderer rend;
    private GameObject self;
    private float baseForce = 0.5f;
    private float difficulty = 1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<Renderer>();
        self = GetComponent<GameObject>();
    }


    public void InitializeEnemy(float difficultyFactor, float size = 1f)
    {
        difficulty = difficultyFactor;
        transform.localScale = Vector3.one * size;
        rb.mass = Mathf.Clamp(size * 1.0f, 0.1f, 20f);
        if (rend != null) rend.material.SetFloat("_Difficulty", difficulty);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (LevelController.instance != null && LevelController.instance.isGameOver) return;
        if (PlayerController.player == null) return;

        Vector3 dir = PlayerController.player.transform.position - transform.position;
        dir.y = 0f;
        dir.Normalize();
        rb.AddForce(dir * baseForce * difficulty, ForceMode.Force);

        if(transform.position.y < -5f)
        {
            Debug.Log("test12");
            Destroy(self);
            LevelController.instance.EnemyOutOfBound();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var pc = collision.gameObject.GetComponent<PlayerController>();
            if (pc != null) pc.HitEffect();
        }
    }
}
