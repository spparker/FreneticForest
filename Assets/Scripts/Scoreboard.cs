using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _baaValue;
    [SerializeField] private TextMeshProUGUI _pattaValue;
    [SerializeField] private TextMeshProUGUI _mashaValue;

    [SerializeField] private TextMeshProUGUI _sizeValue;
    [SerializeField] private TextMeshProUGUI _wisdomValue;
    [SerializeField] private TextMeshProUGUI _happinessValue;


    // Update is called once per frame
    void LateUpdate()
    {
        _baaValue.text = CritterManager.Instance.ChopChopCount.ToString();
        _pattaValue.text = CritterManager.Instance.DiggieCount.ToString();
        _mashaValue.text = CritterManager.Instance.PatherCount.ToString();

        _sizeValue.text = "1";
        _wisdomValue.text = "0";
        _happinessValue.text= "0";
    }
}
