using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bala : Ataque
{
    [Header("Movimiento")]
    [SerializeField] private float _speed = 1.0f;
    [SerializeField] private float _tiempoVida = 5f;

    [Header("Rebote")]
    [SerializeField] private int _rebotes = 0;
    [SerializeField] private LayerMask _capaParedes;
    [SerializeField] private float _radio = 0.1f;

    private Rigidbody2D _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        _rb.linearVelocity = transform.up * _speed;
        Destroy(gameObject, _tiempoVida);
    }

    public void MultiplicadorVelocidad(float factor) 
    {
        _speed *= factor;
    }

    private void FixedUpdate()
    {
        if (_rebotes <= 0)
        {
            return;
        }

        Vector2 velocity = _rb.linearVelocity;
        float distancia = velocity.magnitude * Time.fixedDeltaTime;
        float radio = _radio * transform.localScale.x;
        RaycastHit2D hit = Physics2D.CircleCast(_rb.position, radio, velocity.normalized, distancia, _capaParedes);

        if (hit.collider == null)
        {
            return;
        }

        Vector2 nueva = Vector2.Reflect(velocity, hit.normal);

        _rb.position = hit.centroid + hit.normal * 0.01f;
        _rb.linearVelocity = nueva;
        _rb.rotation = Mathf.Atan2(nueva.y, nueva.x) * Mathf.Rad2Deg - 90f;
        _rebotes--;



    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.GetComponentInParent<Ataque>() != null)
        {
            return;
        }

        Personaje personaje = other.GetComponentInParent<Personaje>();

        if (personaje == null)
        {
            if (!other.isTrigger && _rebotes <= 0)
            {

                Destroy(gameObject);
                return;
            }
        }
        if (IntentarDanar(personaje))
        {

            Destroy(gameObject);
        }
    }
}
