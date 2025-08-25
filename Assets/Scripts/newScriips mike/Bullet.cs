using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float life = 3f;

    private void Start() { Destroy(gameObject, life); }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // aquí podrías dañar enemigos
        Destroy(gameObject);
    }
}
