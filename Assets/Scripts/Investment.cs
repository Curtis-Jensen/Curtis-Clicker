using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Investment : MonoBehaviour
{
    public float cost;
    public float payout;

    string serviceName;
    CentralCalculator calculator;
    Button button;

    /* Finds the main text of the button by looking for the text object
     * 
     * Finds the central calculator by searching for it's name
     */
    void Start()
    {
        serviceName = gameObject.name;
        button = gameObject.GetComponent<Button>();
        gameObject.GetComponentInChildren<Text>().text = serviceName + $"\nCost: {cost:C}\nGold per second: {payout:C}";
        calculator = GameObject.Find("Central Calculator").GetComponent<CentralCalculator>();
    }

    void Update()
    {
        bool affordable = calculator.gold > cost;
        button.interactable = affordable;
    }

    public void BuyPIE()
    {
        Debug.Log(serviceName + "s purchased.");

        calculator.gold -= cost;
        calculator.goldPerSecond += payout;
        calculator.DisplayGold();
    }
}
