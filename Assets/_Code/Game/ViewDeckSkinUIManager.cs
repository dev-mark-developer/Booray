using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ViewDeckSkinUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Button CutButton;
    public Button BackButton;

    public Canvas ProfileCanvas;
    public Canvas DeckSkinCanvas;

    void Start()
    {
        CutButton.onClick.AddListener(CutMethod);
        BackButton.onClick.AddListener(BackMethod);

    }
    public void BackMethod()
    {
        ProfileCanvas.sortingOrder = 10;
        DeckSkinCanvas.sortingOrder = -1;
    }
    public void CutMethod()
    {
      
        DeckSkinCanvas.sortingOrder = -1;
    }
  
}
