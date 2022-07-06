using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExitStayUIController : MonoBehaviour
{
    
    [SerializeField]
    Image greenImage;
    
    private Action onEndTimer;
    private float exitTime;
    private float currentTime;
    
    
    public void StartTimer(float time,Action onEnd)
    {
        
        onEndTimer = onEnd;
        exitTime = time;
        gameObject.SetActive(true);
        Vector3 v = greenImage.rectTransform.localScale;
        v.x = 0;
        greenImage.rectTransform.localScale = v;
    }
    
    public void CancelTimer()
    {
        gameObject.SetActive(false);
        currentTime = 0;
        exitTime = 0;
        onEndTimer = () => { };
        Vector3 v = greenImage.rectTransform.localScale;
        v.x = 1;
        greenImage.rectTransform.localScale = v;
    }
    
    private void Update()
    {
        float percent = currentTime / exitTime;
        Vector3 v = greenImage.rectTransform.localScale;
        v.x = Mathf.Clamp(1 - percent, 0,1);
        greenImage.rectTransform.localScale = v;
        currentTime += Time.deltaTime;
        if (currentTime >= exitTime)
        {
            if (onEndTimer != null)
                onEndTimer();
            
        }
    }


}