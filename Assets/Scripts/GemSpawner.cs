using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject[] gemList;
    public int gemsPerHigherGem = 10;
    public float coalesceSpeed = 1.0f;
    public float buttonDelay = 2.0f;

    private int gemCount = 0;
    private List<GameObject> gemsToDestroy = new List<GameObject>();
    private bool canPressButton = true;

    public void SpawnGem()
    {
        if (!canPressButton) return;
        canPressButton = false;
        StartCoroutine(ButtonDelay());

        Instantiate(gemList[0], GetRandomSpawnPosition(), Quaternion.identity, gameObject.transform);
        gemCount++;

        if (gemCount >= gemsPerHigherGem)
        {
            CoalesceToHigherGem();
            gemCount = 0;
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = UnityEngine.Random.Range(-4f, 4f);
        return new Vector3(x, gameObject.transform.position.y, 0f);
    }

    private void CoalesceToHigherGem()
    {
        Vector3 higherGemPosition = GetRandomSpawnPosition();

        // Move all gems towards the higher gem position and add them to the destroy list
        foreach (Transform gem in gameObject.transform)
        {
            if (gem.gameObject.CompareTag("Gem"))
            {
                StartCoroutine(MoveGemTowardsCenter(gem.gameObject, higherGemPosition));
                gemsToDestroy.Add(gem.gameObject);
            }
        }

        // Wait for the gems to coalesce before destroying them and creating the higher gem
        StartCoroutine(WaitForCoalesceAndCreateGem(higherGemPosition));
    }

    private IEnumerator MoveGemTowardsCenter(GameObject gem, Vector3 higherGemPosition)
    {
        float t = 0.0f;
        Vector3 startPos = gem.transform.position;
        while (t < coalesceSpeed)
        {
            t += Time.deltaTime;
            gem.transform.position = Vector3.Lerp(startPos, higherGemPosition, t / coalesceSpeed);
            yield return null;
        }
    }

    private IEnumerator WaitForCoalesceAndCreateGem(Vector3 higherGemPosition)
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
        gemsToDestroy.Clear();

        // Instantiate the higher gem
        Instantiate(gemList[1], higherGemPosition, Quaternion.identity, gameObject.transform);
    }

    private IEnumerator ButtonDelay()
    {
        yield return new WaitForSeconds(buttonDelay);
        canPressButton = true;
    }
}
