using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject[] gemList;
    public int gemsPerHigherGem = 10;
    public float coalesceSpeed = 1.0f;
    public float spreadAmount = 6f;

    int[] gemCounts;
    List<GameObject> gemsToDestroy = new List<GameObject>();
    bool canPressButton = true;

    void Start()
    {
        gemCounts = new int[gemList.Length];
    }

    public void SpawnGemButton(float buttonDelay)
    {
        if (!canPressButton) return;
        canPressButton = false;
        StartCoroutine(ButtonDelay(buttonDelay));

        SpawnGem(0);
    }

    IEnumerator ButtonDelay(float buttonDelay)
    {
        yield return new WaitForSeconds(buttonDelay);
        canPressButton = true;
    }

    void SpawnGem(int index)
    {
        var newSpawnPosition = GetRandomSpawnPosition();

        Instantiate(gemList[index], newSpawnPosition, Quaternion.identity, gameObject.transform);

        if (index > gemList.Length - 1) return;
        gemCounts[index]++;

        if (gemCounts[index] >= gemsPerHigherGem)
        {
            gemCounts[index] = 0;
            StartCoroutine(CoalesceToHigherGem(newSpawnPosition, index + 1));

            //SpawnGem(index + 1);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        var parentPosition = gameObject.transform.position;
        float x = Random.Range(parentPosition.x - spreadAmount, parentPosition.x + spreadAmount);
        return new Vector3(x, parentPosition.y, 0f);
    }

    IEnumerator CoalesceToHigherGem(Vector2 higherGemPosition, int higherGemIndex)
    {
        List<GameObject> children = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            GameObject childGameObject = childTransform.gameObject;
            children.Add(childGameObject);
        }

        // Move all gems towards the higher gem position and add them to the destroy list
        foreach (GameObject gem in children)
        {
            if (gem.CompareTag("Gem"))
            {
                StartCoroutine(MoveGemTowardsCenter(gem.gameObject, higherGemPosition));
                gemsToDestroy.Add(gem.gameObject);
            }
        }

        // Wait for the gems to coalesce before destroying them and creating the higher gem
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

        // Create the higher gem
        Instantiate(gemList[higherGemIndex], higherGemPosition, Quaternion.identity, gameObject.transform);
    }

    IEnumerator MoveGemTowardsCenter(GameObject gem, Vector3 higherGemPosition)
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
}
