using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject gem;
    public GameObject squarePrefab;
    public float spawnRadius = 2.0f;
    public int gemsPerSquare = 100;

    private GameObject gemParent;

    private void Start()
    {
        gemParent = new GameObject("Gems");
    }

    public void SpawnGem()
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        Instantiate(gem, spawnPosition, Quaternion.identity, gemParent.transform);

        int numGems = gemParent.transform.childCount;
        if (numGems >= gemsPerSquare)
        {
            CoalesceGemsIntoSquare();
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = transform.position + new Vector3(randomCircle.x, randomCircle.y, 0);
        return spawnPosition;
    }

    private void CoalesceGemsIntoSquare()
    {
        Vector3 centerPosition = GetRandomSpawnPosition();
        Rigidbody2D[] rigidbodies = gemParent.GetComponentsInChildren<Rigidbody2D>();
        Collider2D[] colliders = gemParent.GetComponentsInChildren<Collider2D>();
        int numGems = rigidbodies.Length;

        for (int i = 0; i < numGems; i++)
        {
            Rigidbody2D rb = rigidbodies[i];
            Collider2D collider = colliders[i];
            if (collider.gameObject != squarePrefab)
            {
                Vector3 targetPosition = centerPosition + (Vector3)UnityEngine.Random.insideUnitCircle * 0.5f;
                StartCoroutine(MoveToTarget(rb, targetPosition, () =>
                {
                    Destroy(collider);
                    Destroy(rb.gameObject);
                }));
            }
        }

        GameObject square = Instantiate(squarePrefab, centerPosition, Quaternion.identity);
        square.transform.parent = gemParent.transform;
    }

    private IEnumerator MoveToTarget(Rigidbody2D rb, Vector3 targetPosition, System.Action onComplete)
    {
        float duration = 0.5f;
        float elapsed = 0.0f;
        Vector3 startPosition = rb.transform.position;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            rb.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        onComplete?.Invoke();
    }

}
