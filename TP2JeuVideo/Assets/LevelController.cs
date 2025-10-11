using System.Collections;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;
    public GameObject enemyPrefab;
    public GameObject[] powerUpPrefab;
    public bool gameOver = false;
    public int wave = 0;
    public float difficulty = 1f;
    public bool hasExtraLife = false;
    public int baseEnemiesPerWave = 3;
    public int currentEnemyCount = 0;
    public float arenaRadius = 8f;
    private float enemyGlobalMultiplier = 1f;
    private Coroutine multiplierCoroutine;
    private RotateCamera cameraController;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        NextWave();
    }

    private void NextWave()
    {
        wave++;
        // Calcul de difficult : 1.0 + 0.25 * wave (ex: wave1 => 1.25)
        difficulty = 1f + wave * 0.50f;

        int enemiesToSpawn = baseEnemiesPerWave + wave - 1;
        currentEnemyCount = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector2 r = Random.insideUnitCircle * arenaRadius;
            Vector3 pos = new Vector3(r.x, 0.5f, r.y);
            if (enemyPrefab == null)
            {
                Debug.LogError("LevelController: enemyPrefab is not assigned!");
                return;
            }

            GameObject e = Instantiate(enemyPrefab, pos, Quaternion.identity);
            var ec = e.GetComponent<EnemyController>();
            if (ec == null)
            {
                Debug.LogWarning("Spawned enemy has no EnemyController attached.");
                continue;
            }

            float size = Random.Range(0.8f, 1.2f) * (1f + wave * 0.08f);
            ec.InitializeEnemy(size);
        }

        // --- PowerUp Spawn Logic ---
        if (powerUpPrefab != null && powerUpPrefab.Length > 0)
        {
            // Determine how many power-ups to spawn this wave
            int powerUpCount = GetPowerUpCountForWave(wave);

            for (int i = 0; i < powerUpCount; i++)
            {
                Vector2 r2 = Random.insideUnitCircle * arenaRadius;
                Vector3 spawnPos = new Vector3(r2.x, 0.5f, r2.y);

                // Choose which power-up types are unlocked based on the wave number
                int maxUnlocked = Mathf.Min(powerUpPrefab.Length, 1 + (wave / 3)); // new type every 3 waves
                int randomIndex = Random.Range(0, maxUnlocked);

                Instantiate(powerUpPrefab[randomIndex], spawnPos, Quaternion.identity);
            }
        }


    }
    private int GetPowerUpCountForWave(int currentWave)
    {
        if (currentWave < 3) return 1;
        if (currentWave < 6) return 2;
        return 3;
    }



    public void EnemyOutOfBound()
    {
        // Don't start a new wave if the game is already over
        if (gameOver) return;

        currentEnemyCount--;
        if (currentEnemyCount <= 0)
        {
            NextWave();
        }
    }

    public void GameOver()
    {
        if (hasExtraLife && PlayerController.player != null)
        {
            hasExtraLife = false;
            // respawn example
            PlayerController.player.transform.position = new Vector3(0, 1, 0);
            if (PlayerController.player.TryGetComponent<Rigidbody>(out var r))
            {
                r.linearVelocity = Vector3.zero;
            }
        }
        else
        {
            // Prevent this from being called more than once
            if (gameOver) return;

            gameOver = true;
            Debug.Log("Game Over! Starting wave spam sequence.");

            // --- NEW CODE STARTS HERE ---

            // 1. Find the camera and tell it to start spinning
            cameraController = GameObject.FindWithTag("FocalPoint").GetComponent<RotateCamera>();
            if (cameraController != null)
            {
                cameraController.autoRotate = true;
            }
            else
            {
                Debug.LogWarning("Could not find a RotateCamera script on the main camera.");
            }

            // 2. Start the coroutine to spam waves
            StartCoroutine(GameOverWaveSpam());

            // --- NEW CODE ENDS HERE ---
        }
    }

    // --- NEW COROUTINE ---
    private IEnumerator GameOverWaveSpam()
    {
        // Loop forever
        while (true)
        {
            // Call the existing function to spawn a new wave
            NextWave();
            // Wait for a short time before spawning the next one
            yield return new WaitForSeconds(0.75f);
        }
    }


    public void SetEnemyGlobalMultiplier(float mult, float duration)
    {
        if (multiplierCoroutine != null) StopCoroutine(multiplierCoroutine);
        multiplierCoroutine = StartCoroutine(TemporaryMultiplier(mult, duration));
    }

    private IEnumerator TemporaryMultiplier(float mult, float duration)
    {
        enemyGlobalMultiplier = mult;
        yield return new WaitForSeconds(duration);
        enemyGlobalMultiplier = 1f;
    }
}