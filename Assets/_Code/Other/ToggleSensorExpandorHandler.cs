 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ToggleSensorExpandorHandler : MonoBehaviour
{
    [SerializeField] private Toggle targetTgl;

    [SerializeField] private Button sensorExpander;


    private void Start()
    {
        sensorExpander.onClick.AddListener(OnSensorExpanderClicked);

    }


    public void OnSensorExpanderClicked()
    {

        Debug.Log(" OnSensorExpanderClicked");

        targetTgl.isOn = !targetTgl.isOn;
    }


}
