using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class TargetMarker : MonoBehaviour
{
    [SerializeField] Transform current_target;
    [SerializeField] float offset = 0;
    [SerializeField] float movement_range = 2.0f;
    [SerializeField] float movement_duration = 2.0f;
    DG.Tweening.Sequence animation_sequence = null;

    float t = 0.0f;

    public Transform TARGET => current_target;

    private void Start()
    {
        FollowTarget(current_target, offset);
    }
       
    public void FollowTarget(Transform target, float target_offset, float range = 1.0f, float duration = 0.5f)
    {
        animation_sequence?.Kill();
        animation_sequence = null;

        if (target == null) return;
        offset = target_offset;
        movement_range = range;
        movement_duration = duration;
        current_target = target;

        animation_sequence = DOTween.Sequence();
        animation_sequence.Append(
            DOTween.To(() => t, x => t = x, movement_range, movement_duration).From(offset)
        );
        animation_sequence.SetLoops(int.MaxValue, LoopType.Yoyo);

    }

    private void FixedUpdate()
    {
        if (current_target == null || current_target.IsDestroyed())
        {
            current_target = null;
            gameObject.SetActive(false);
            return;
        }

        transform.position = current_target.position + Vector3.one * t;
    }

    private void OnDestroy()
    {
        animation_sequence?.Kill();
    }
}
