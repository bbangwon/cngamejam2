using UnityEngine;
using DG.Tweening;

public class bullet : MonoBehaviour
{
    [SerializeField] float shotDuration = 0.3f;
     
    public void SetStartPosition(Vector3 startPos)
    {
        transform.position = startPos;
    }

    public void Shot(float attackRange)
    {
        GetComponent<SpriteRenderer>().flipX = attackRange < 0;
        transform.DOMoveX(transform.position.x + attackRange, shotDuration).OnComplete(() => Destroy(gameObject));
    }
}
