using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Booray.Game;
public class MiddleStackUIHandler : MonoBehaviour
{

    public MiddleStackDroppableAreaHandler droppableAreaHandler;

    public GameObject middleStackSensorPanel;

    public void OpenMiddleStackSensor()
    {
        droppableAreaHandler.resetSensorColor();
        middleStackSensorPanel.SetActive(true);

    }

    public void CloseMiddleStackSensor()
    {
        droppableAreaHandler.resetSensorColor();
        middleStackSensorPanel.SetActive(false);
    }
}
