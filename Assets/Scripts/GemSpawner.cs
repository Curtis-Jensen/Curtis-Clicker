using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject gemPrefab;
    public GameObject squarePrefab;
    public int gemsPerSquare = 100;
    public float coalesceSpeed = 1.0f;

    private int gemCount = 0;
    private List<GameObject> gemsToDestroy = new List<GameObject>();

    public void SpawnGem()
    {
        Instantiate(gemPrefab, GetRandomSpawnPosition(), Quaternion.identity, gameObject.transform);
        gemCount++;

        if (gemCount >= gemsPerSquare)
        {
            CoalesceGemsAndCreateSquare();
            gemCount = 0;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = UnityEngine.Random.Range(-4f, 4f);
        return new Vector3(x, gameObject.transform.position.y, 0f);
    }

    private void CoalesceGemsAndCreateSquare()
    {
        Vector3 squarePosition = GetRandomSpawnPosition();

        // Move all gems towards the square position and add them to the destroy list
        foreach (Transform gem in gameObject.transform)
        {
            if (gem.gameObject.CompareTag("Gem"))
            {
                StartCoroutine(MoveGemTowardsSquare(gem.gameObject, squarePosition));
                gemsToDestroy.Add(gem.gameObject);
            }
        }

        // Wait for the gems to coalesce before destroying them and creating the square
        StartCoroutine(WaitForCoalesceAndCreateSquare(squarePosition));
    }

    private IEnumerator MoveGemTowardsSquare(GameObject gem, Vector3 squarePosition)
    {
        float t = 0.0f;
        Vector3 startPos = gem.transform.position;
        while (t < coalesceSpeed)
        {
            t += Time.deltaTime;
            gem.transform.position = Vector3.Lerp(startPos, squarePosition, t / coalesceSpeed);
            yield return null;
        }
    }

    private IEnumerator WaitForCoalesceAndCreateSquare(Vector3 squarePosition)
    {
        yield return new WaitForSeconds(coalesceSpeed);

        // Destroy all the gems in the destroy list
        foreach (GameObject gem in gemsToDestroy)
        {
            if (gem != null)
            {
                Collider2D gemCollider = gem.GetComponent<Collider2D>();
                if (gemCollider != null)
                    Destroy(gemCollider);
                Rigidbody2D gemRigidbody = gem.GetComponent<Rigidbody2D>();
                if (gemRigidbody != null)
                    Destroy(gemRigidbody);
                Destroy(gem);
            }
        }

        // Instantiate the square
        Instantiate(squarePrefab, squarePosition, Quaternion.identity, gameObject.transform);
    }
}
