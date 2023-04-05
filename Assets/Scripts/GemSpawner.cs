using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject[] gemList;
    public int gemsPerHigherGem = 10;
    public float coalesceSpeed = 1.0f;
    public float buttonDelay;

    private int[] gemCounts;
    private List<GameObject> gemsToDestroy = new List<GameObject>();
    private bool canPressButton = true;

    private void Start()
    {
        gemCounts = new int[gemList.Length];
    }

    /* 1 Wait for the button press
     * 
     * 2 Instantiate a gem of the given index */
    public void SpawnGem(int index)
    {
        // Commented out because recursion doesn't work, even when the button delay is 0.  Probably need this part of gemspawning to be the gem spawn button.

        //if (!canPressButton) return; // 1
        //canPressButton = false;
        //StartCoroutine(ButtonDelay());

        var newSpawnPosition = GetRandomSpawnPosition(); // 2

        Instantiate(gemList[index], newSpawnPosition, Quaternion.identity, gameObject.transform);
        gemCounts[index]++;

        if (gemCounts[index] >= gemsPerHigherGem && index < gemList.Length - 1) // This might need to be just "index < gemList.Length - 0"
        {
            gemCounts[index] = 0;
            StartCoroutine(CoalesceToHigherGem(newSpawnPosition, index + 1));
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = UnityEngine.Random.Range(-4f, 4f);
        return new Vector3(x, gameObject.transform.position.y, 0f);
    }

    private IEnumerator CoalesceToHigherGem(Vector2 higherGemPosition, int higherGemIndex)
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

    private IEnumerator ButtonDelay()
    {
        yield return new WaitForSeconds(buttonDelay);
        canPressButton = true;
    }
}
