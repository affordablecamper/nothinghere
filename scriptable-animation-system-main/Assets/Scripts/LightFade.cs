using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFade : MonoBehaviour
{
    public Light lt;
    public float fadeSpeed = 2f;
    public float highIntensity = 6f;
    public float lowIntensity = 0.0f;
    private float targetIntensity;

    private void Update()
    {

        lt.intensity = Mathf.Lerp(lt.intensity, targetIntensity, fadeSpeed * Time.deltaTime);



    }


}
