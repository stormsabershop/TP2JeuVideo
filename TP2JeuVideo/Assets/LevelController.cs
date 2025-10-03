using System;
using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class LevelController : MonoBehaviour
{
    public static LevelController instance;
    public GameObject enemyPrefab;
    public GameObject powerUpPrefab;
    public bool isGameOver = false;
    public int wave = 0;
    public float difficulty = 1f;
    public bool hasExtraLife = false;
    public int baseEnemiesPerWave = 3;
    public int currentEnemyCount = 0;
    public float arenaRadius = 8f;
    private float enemyGlobalMultiplier = 1f;
    private Coroutine multiplierCoroutine;
 

    void Awake()
    {
        if (instance != null && instance != this) Destroy(this.gameObject);
        else instance = this;
    }
    private void Start()
    {
        NextWave();
    }

    private void NextWave()
    {
        wave++;
        difficulty = 1f * wave * 0.25f;
        int enemiesToSpawn = baseEnemiesPerWave + wave - 1;
        currentEnemyCount = enemiesToSpawn;

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Vector2 r = UnityEngine.Random.insideUnitCircle * arenaRadius;
            Vector3 pos = new Vector3(r.x, 0.5f, r.y);
            GameObject e = Instantiate(enemyPrefab, pos, Quaternion.identity);
            var ec = e.GetComponent<EnemyController>();
            float size = UnityEngine.Random.Range(0.8f, 1.2f) * (1f + wave * 0.08f);
            ec.InitializeEnemy(difficulty * enemyGlobalMultiplier, size);

        }
        if (powerUpPrefab != null)
        {
            Vector2 r2 = UnityEngine.Random.insideUnitCircle * arenaRadius;
            Vector3 pp = new Vector3(r2.x, 0.5f, r2.y);
            Instantiate(powerUpPrefab, pp, Quaternion.identity);
        }

    }

    public void EnemyOutOfBound()
    {
        currentEnemyCount--;
        if (currentEnemyCount <= 0 && !isGameOver)
        {
            NextWave();
        }
    }

    public void GameOver()
    {
        if (hasExtraLife)
        {
            hasExtraLife = false;
            // respawn example
            PlayerController.player.transform.position = new Vector3(0, 1, 0);
            if (PlayerController.player.TryGetComponent<Rigidbody>(out var r)) { r.linearVelocity = Vector3.zero; }
        }
        else
        {
            isGameOver = true;
            Debug.Log("GameOver");
            // play sound / show UI
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
