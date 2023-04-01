using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CentralCalculator : MonoBehaviour
{
    public float gold;
    public float goldPerSecond;
    public Text goldText;
    public Text  goldPerSecondText;

    float seconds;

    /* If the gold is set to 0 in the inspector it means that it should bring the saved amount
     */
    void Start()
    {
        if(gold == 0)
            gold = PlayerPrefs.GetFloat("gold");

        DisplayGold();
    }

    void Update()
    {
        seconds += Time.deltaTime;
        if (seconds > 1)
        {
            gold += goldPerSecond;
            DisplayGold();
            seconds = 0;
        }
    }

    internal void DisplayGold()
    {
        goldText.text = gold.ToString("C") + " Gold";
        goldPerSecondText.text = "per second: " + goldPerSecond.ToString("C");
    }
}
