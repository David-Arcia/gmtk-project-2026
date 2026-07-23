using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    public GameObject enemyPrefab;
    [SerializeField]
    public List<Vector2> positions;
    [SerializeField]
    public float cooldown = 2;
    private float cooldownTimer = 0;
    private Dictionary<Vector2, GameObject> enemyTracker;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyTracker = new Dictionary<Vector2, GameObject>();
        foreach (Vector2 pos in positions)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
            enemyTracker.Add(pos, newEnemy);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= cooldown)
        {
            cooldownTimer = 0;
            List<Vector2> keysChanged = new List<Vector2>();
            List<GameObject> newOBjs = new List<GameObject>();
            foreach (KeyValuePair<Vector2, GameObject> kvp in enemyTracker)
            {
                if (!kvp.Value)
                {
                    GameObject newEnemy = Instantiate(enemyPrefab, kvp.Key, Quaternion.identity);
                    keysChanged.Add(kvp.Key);
                    newOBjs.Add(newEnemy);
                }
            }
            for (int i = 0; i < keysChanged.Count; i++) {
                enemyTracker[keysChanged[i]] = newOBjs[i];
            }
        }
    }
}
