using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcessGoods : MonoBehaviour
{
    public float cost;
    public float payout;
    public float processingTime;

    string processingName;
    CentralCalculator calculator;
    Button processButton;

    /* Finds the main text of the button by looking for the text object
     * Finds the central calculator by searching for its name
     */
    void Start()
    {
        processingName = gameObject.name;
        processButton = gameObject.GetComponent<Button>();
        gameObject.GetComponentInChildren<Text>().text = processingName + $"\nCost: {cost:C}\nGold per process: {payout:C}";
        calculator = GameObject.Find("Central Calculator").GetComponent<CentralCalculator>();
    }

    void Update()
    {
        bool affordable = calculator.gold > cost;
        processButton.interactable = affordable;
    }

    public void Process()
    {
        StartCoroutine(ProcessingCoroutine());
    }

    IEnumerator ProcessingCoroutine()
    {
        Debug.Log(processingName + " in progress...");

        calculator.gold -= cost;
        calculator.DisplayGold();

        yield return new WaitForSeconds(processingTime);

        calculator.gold += payout;
        calculator.DisplayGold();

        Debug.Log(processingName + " completed. Earned: " + payout);
    }
}
