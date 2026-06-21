using UnityEngine;

public class RandomFolliageSelector : MonoBehaviour
{

    [SerializeField] Sprite[] FolliageSprite;

    private void Awake()
    {
        Sprite sprite = FolliageSprite[Random.Range(0, FolliageSprite.Length)];
        GetComponent<SpriteRenderer>().sprite = sprite;
    }

}
