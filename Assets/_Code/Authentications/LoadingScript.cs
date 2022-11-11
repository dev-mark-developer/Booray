using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LoadingScript : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Loader;
    

    public Transform imageScale;

    private Tween arrowTween;
    private Tween logoTween;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        logoTween = imageScale.DOScale(1.25f, 5).SetLoops(-1,LoopType.Yoyo).SetEase(Ease.InOutCubic);
        arrowTween = Loader.transform.DORotate(new Vector3(0, 0, -360), 5f, RotateMode.LocalAxisAdd).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    private void OnDisable()
    {
        logoTween.Kill();
        imageScale.localScale = Vector3.one;
        arrowTween.Kill();
    }



}
