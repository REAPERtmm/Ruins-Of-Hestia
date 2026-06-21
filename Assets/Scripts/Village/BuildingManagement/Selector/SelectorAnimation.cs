using System;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectorAnimation : MonoBehaviour
{

    public float TransitionTime = 0.3f;
    public float SizeMulti = 1.2f;
    public float DirectionMulti = 1.2f;
    [FormerlySerializedAs("direction")] public Vector3 Direction;
    private Vector3 startSize;

    private Sequence scaleSequence;
    private Sequence positionSequence;

    private void Start()
    {
        startSize = transform.localScale;

        scaleSequence = DOTween.Sequence();
        scaleSequence.Append(transform.DOScale(startSize * SizeMulti, TransitionTime));
        scaleSequence.Append(transform.DOScale(startSize, TransitionTime));
        scaleSequence.SetLoops(int.MaxValue, LoopType.Yoyo);
    }

    void OnEnable()
    {
        DOTween.Restart(scaleSequence);
    }

    private void OnDisable()
    {
        DOTween.Pause(scaleSequence);
        DOTween.Pause(positionSequence);
    }

    public void UpdatePosition( Vector3 newPos )
    {
        transform.transform.localPosition = newPos;

        positionSequence?.Kill();
        positionSequence = DOTween.Sequence();
        positionSequence.Append(transform.DOMove(transform.position + (Direction * DirectionMulti), TransitionTime));
        positionSequence.SetLoops(int.MaxValue, LoopType.Yoyo);
        DOTween.Restart(positionSequence);
    }
}
