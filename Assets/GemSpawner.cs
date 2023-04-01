using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject gemPrefab;
    public GameObject squarePrefab;
    public Transform gemParent;
    public int gemsPerSquare = 100;

    private int gemCount = 0;

    public void SpawnGem()
    {
        Instantiate(gemPrefab, GetRandomSpawnPosition(), Quaternion.identity, gemParent);
        gemCount++;

        if (gemCount >= gemsPerSquare)
        {
            DestroyAllGemsExceptSquare();
            CreateSquareGem();
            gemCount = 0;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = UnityEngine.Random.Range(-4f, 4f);
        return new Vector3(x, gameObject.transform.position.y, 0f);
    }

    private void DestroyAllGemsExceptSquare()
    {
        foreach (Transform gem in gemParent)
            if (gem.gameObject.CompareTag("Gem"))
                Destroy(gem.gameObject);
    }

    private void CreateSquareGem()
    {
        Instantiate(squarePrefab, GetRandomSpawnPosition(), Quaternion.identity, gemParent);
    }
}
