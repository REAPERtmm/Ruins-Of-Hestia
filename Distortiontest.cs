using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteAlways]
public class Distortiontest : MonoBehaviour
{
    [SerializeField] float Duration;
    [SerializeField] float Intensity;
    [SerializeField] Volume PostEffect;
    [SerializeField] AnimationCurve Curve;
    [SerializeField] bool EXECUTE;
    [SerializeField] bool EXECUTEINVERSE;
    [SerializeField] bool STOP;

    Coroutine current = null;

    IEnumerator DistortionCoroutine()
    {
        LensDistortion distortion;
        PostEffect.profile.TryGet(out distortion);

        float TimeStep = Duration / 50.0f;
        for(int i = 0; i < 50; ++i)
        {
            float v = Curve.Evaluate(i / 49.0f);

            distortion.intensity.value = -v;
            distortion.scale.value = 1 - v;

            yield return new WaitForSeconds(TimeStep);
        }

        current = null;
    }

    IEnumerator InverseDistortionCoroutine()
    {
        LensDistortion distortion;
        PostEffect.profile.TryGet(out distortion);

        float TimeStep = Duration / 50.0f;
        for (int i = 0; i < 50; ++i)
        {
            float v = Curve.Evaluate(1 - (i / 49.0f));

            distortion.intensity.value = -v;
            distortion.scale.value = 1 - v;

            yield return new WaitForSeconds(TimeStep);
        }

        current = null;
    }

    private void Update()
    {
        if (EXECUTE)
        {
            if (current == null) { 
                current = StartCoroutine(DistortionCoroutine());
            }
            EXECUTE = false;
        }
        if (EXECUTEINVERSE)
        {
            if (current == null)
            {
                current = StartCoroutine(InverseDistortionCoroutine());
            }
            EXECUTEINVERSE = false;
        }
        if (STOP)
        {
            if (current != null)
            {
                StopCoroutine(current);
            }
            current = null;
            STOP = false;
        }
    }
}
