using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public GameObject gameManager;
    public GameObject holder;

    public Vector3 center;
    public Vector3 size;

    public GameObject prefab;

    public bool goodAi;
    public bool badAi;
    public bool ball;

    public Color spawnColor;

    private void Awake()
    {
        gameManager = GameObject.Find("Game Manager");
        holder = GameObject.Find("Ai Holder");
    }

    // Start is called before the first frame update
    void Start()
    {
        SpawnAi();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnAi()
    {
        int spawnAmount = 0;
        if (goodAi)
        {
            spawnAmount = gameManager.GetComponent<SpawnManagement>().goodAiSpawnAmount;

        } else if (badAi)
        {
            spawnAmount = gameManager.GetComponent<SpawnManagement>().badAiSpawnAmount;
        } else if (ball)
        {
            spawnAmount = gameManager.GetComponent<SpawnManagement>().ballSpawnAmount;
            holder = GameObject.Find("Ball Holder");
        }
        int i = 0;
        while (i < spawnAmount) {
            Vector3 pos = center + new Vector3(Random.Range(-size.x / 2, size.x / 2), Random.Range(-size.y / 2, size.y / 2), Random.Range(-size.z / 2, size.z / 2));
            GameObject childAi = Instantiate(prefab, pos, Quaternion.identity);
            childAi.transform.parent = holder.transform;
            int f = gameManager.GetComponent<NameGenerator>().firstName.Length;
            int l = gameManager.GetComponent<NameGenerator>().lastName.Length;
            childAi.name = gameManager.GetComponent<NameGenerator>().firstName[Random.Range(0, f)] + " " + gameManager.GetComponent<NameGenerator>().lastName[Random.Range(0, l)];
            if (goodAi) gameManager.GetComponent<GameStatus>().friendlyCount += 1;
            else if (badAi) gameManager.GetComponent<GameStatus>().enemyCount += 1;
            i++;
        }

        gameManager.GetComponent<GameStatus>().SetText(gameManager.GetComponent<GameStatus>().friendlyCount, 
            gameManager.GetComponent<GameStatus>().enemyCount);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(spawnColor.r, spawnColor.g, spawnColor.b, .5f);
        Gizmos.DrawCube(center, size);
    }
}
