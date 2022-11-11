using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyPiechartscript : MonoBehaviour
{
    // Start is called before the first frame update
    public Image[] ImagesPiechart;
    public float[] Pievalues;
    void Start()
    {
        SetValues(Pievalues);
    }
    public void SetValues(float[] valuesToSet)
    {
        float totalValues = 0;
        for(int i = 0; i < ImagesPiechart.Length; i++)
        {
            totalValues += FindPercentage(valuesToSet, i);
            ImagesPiechart[i].fillAmount = totalValues;
        }
    }
    private float FindPercentage(float[]valuesToSet, int index)
    {
        float totalAmount = 0;
        for(int i=0; i < valuesToSet.Length; i++)
        {
            totalAmount += valuesToSet[i];

        }
        return valuesToSet[index] / totalAmount;
    }

}
