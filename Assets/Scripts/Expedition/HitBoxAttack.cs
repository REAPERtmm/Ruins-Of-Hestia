using UnityEngine;

public struct HitInstanceDescriptor
{
    public CombatController SourceController;
}

public class HitBoxAttack : MonoBehaviour
{
    [SerializeField] int Penetration = 1;
    [SerializeField] HitInstanceDescriptor HitInstance;
    [SerializeField] Collider SelfCollider;

    Vector3 LastPosition = Vector3.zero;
    bool ShouldFaceDirection;
    float TimeShouldDie;

    public void Init(in HitInstanceDescriptor desc, float scale, float lingeringTime, bool shouldFaceDirection = false)
    {
        HitInstance = desc;

        transform.localScale = Vector3.one * scale;
        TimeShouldDie = Time.time + lingeringTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        string target;
        switch(HitInstance.SourceController.GROUP)
        {
            default:
            case EntityGroup.Ally: target = "Ally"; break;
            case EntityGroup.Ennemi: target = "Ennemi"; break;
        }

        if (collision.transform.tag != target) { 
            CombatController combat = collision.transform.GetComponent<CombatController>();
            if (combat == null) return;

            
        }
        
    }

    private void FixedUpdate()
    {
        if (Time.time > TimeShouldDie) { 
            Destroy(gameObject);
        }
        if(ShouldFaceDirection)
        {
            Vector3 direction = (transform.position - LastPosition).normalized;
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }

        LastPosition = transform.position;
    }
}
