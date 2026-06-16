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
    }

    public void Highlight( ResourceType type )
    {
        _Resources[(int)type].transform.DOScale(MaxScale, AnimationSpeed).SetEase( Ease.InElastic )
            .OnComplete(() =>
            {
                _Resources[(int)type].transform.DOScale(1.0f, AnimationSpeed).SetEase(Ease.OutElastic);
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
