using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasicAnimation : MonoBehaviour
{
    public Vector3 scaleInit;
    public Vector3 scaleEnd;
    public float duration;

    private float timeStarted;

    public void PlayAnimation()
    {
        timeStarted = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time <= timeStarted + duration)
        {
            float advancement = (Time.time - timeStarted) / duration;
            Vector3 currentScale = scaleInit * (1 - advancement) + scaleEnd * advancement;
            transform.localScale = currentScale;
            Color currentColor = GetComponent<RawImage>().color;
            currentColor.a = 1 - advancement; 
            GetComponent<RawImage>().color = currentColor;
        }
        else
        {
            gameObject.SetActive(false);
        }
        
    }
}
