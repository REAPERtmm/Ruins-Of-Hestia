using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ResourceUI : MonoBehaviour
{
    [SerializeField] private float AnimationSpeed;
    [SerializeField] private float MaxScale;

    [SerializeField] private GameObject Wood;
    [SerializeField] private GameObject Stone;
    [SerializeField] private GameObject Metal;
    [SerializeField] private GameObject Leaves;
    [SerializeField] private GameObject Food;

    Vector3 originalScale;
    private GameObject[] _Resources;

    private void Start()
    {
        _Resources = new[] {
            Wood,
            Stone,
            Metal,
            Leaves,
            Food,
        };
        originalScale = _Resources[0].transform.localScale;

    }

    public void Highlight( ResourceType type )
    {
        _Resources[(int)type].transform.DOScale(MaxScale, AnimationSpeed).SetEase( Ease.Linear )
            .OnComplete(() =>
            {
                _Resources[(int)type].transform.DOScale(originalScale, AnimationSpeed).SetEase(Ease.Linear);
            });
    }

    public void Highlight(List<ResourcesCost> price)
    {
        foreach (var resourcesCost in price)
        {
            Highlight(resourcesCost.Type);
        }
    }
}
