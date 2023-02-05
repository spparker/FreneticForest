using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _baaValue;
    [SerializeField] private TextMeshProUGUI _pattaValue;
    [SerializeField] private TextMeshProUGUI _mashaValue;

    [SerializeField] private TextMeshProUGUI _sizeValue;
    [SerializeField] private TextMeshProUGUI _wisdomValue;
    //[SerializeField] private TextMeshProUGUI _happinessValue; // Make this slider bettern 0 -1 = (TotalHappiness / TotalSize)
    [SerializeField] private  Slider _stressSlider;

    // Update is called once per frame
    void LateUpdate()
    {
        if(ForestManager.Instance.HomeNetwork.TotalSize < 1)
            return;
        _baaValue.text = CritterManager.Instance.ChopChopCount.ToString();
        _pattaValue.text = CritterManager.Instance.DiggieCount.ToString();
        _mashaValue.text = CritterManager.Instance.PatherCount.ToString();

        _sizeValue.text = ForestManager.Instance.HomeNetwork.TotalSize + " Trees";
        _wisdomValue.text = Mathf.FloorToInt(ForestManager.Instance.HomeNetwork.TotalWisdom) + " Years";
        //_happinessValue.text= ForestManager.Instance.HomeNetwork.TotalStress.ToString();
        float stress_average = ForestManager.Instance.HomeNetwork.TotalStress / ForestManager.Instance.HomeNetwork.TotalSize;
        _stressSlider.value = stress_average;
    }
}
