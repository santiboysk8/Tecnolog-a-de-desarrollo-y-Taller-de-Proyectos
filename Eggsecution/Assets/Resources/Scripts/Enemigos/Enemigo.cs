using UnityEngine;

// abstract → no existe un "enemigo genérico", solo EnemyMelee, Aceituna, Sal...
public abstract class Enemigo : Personaje
{
    // Todos los enemigos son del bando Enemigo
    public override Bando Bando => Bando.Enemigo;

    [Header("DETECCIÓN")]
    [SerializeField] protected float _distanciaDeteccion = 5f;   // Si un jugador se acerca a menos de esto, le hace focus
    [SerializeField] protected float _distanciaPerderFocus = 8f; // Si el objetivo se aleja más de esto, lo suelta (debe ser MAYOR que la de detección)

    [Header("VISUAL")]
    [SerializeField] protected bool _spriteMiraDerecha = true;   // NUEVO: marca hacia dónde mira el sprite tal y como está dibujado

    protected Transform _objetivo;               // El jugador al que tiene fijado. null = no tiene a nadie
    protected Rigidbody2D _rb;                   // protected → los hijos lo pueden usar
    protected SpriteRenderer _spriteRenderer;    // NUEVO: para girar el sprite

    // Override del Awake de Personaje para coger los componentes
    protected override void Awake()
    {
        if (SeleccionPartida.HayJugador2)
        {
            _vidaMaxima *= 2.0f;
        }
        base.Awake();
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
        _rb.freezeRotation = true;

        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();


    }

    // ---------- HERRAMIENTAS PARA LOS HIJOS ----------

    protected void MoverHacia(Vector2 destino, float velocidad)
    {
        Vector2 dir = (destino - _rb.position).normalized; // Dirección hacia el destino, longitud 1
        _rb.linearVelocity = dir * velocidad;              // La física lo mueve y lo para si hay una pared
    }

    protected void AlejarseDe(Vector2 origen, float velocidad)
    {
        Vector2 dir = (_rb.position - origen).normalized;   // Al revés que MoverHacia
        _rb.linearVelocity = dir * velocidad;
    }

    protected void Detener()
    {
        _rb.linearVelocity = Vector2.zero;
    }

    // NUEVO: gira el sprite hacia una X (un punto de patrulla, el jugador...)
    protected void MirarHacia(float xDestino)
    {
        if (_spriteRenderer == null) return;

        float dif = xDestino - transform.position.x;
        if (Mathf.Abs(dif) < 0.01f) return;   // Misma X → no girar (evita que tiemble)

        bool vaALaDerecha = dif > 0;
        _spriteRenderer.flipX = _spriteMiraDerecha ? !vaALaDerecha : vaALaDerecha;
    }

    // Crea un ataque delante del enemigo, girado hacia el objetivo (golpe o bala)
    protected GameObject CrearHacia(GameObject prefab, Transform objetivo, float distancia)
    {
        if (prefab == null || objetivo == null) return null;

        Vector3 dir = (objetivo.position - transform.position).normalized; // Dirección hacia el objetivo (longitud 1)
        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;   // Para que el eje Y del prefab mire al objetivo
        return CrearAtaque(prefab, transform.position + dir * distancia, Quaternion.Euler(0, 0, angulo)); // Heredado de Personaje: pone el bando
    }

    // Atajo para buscar al jugador más cercano
    protected Transform BuscarJugadorCercano()
    {
        return PlayerMovement.JugadorMasCercano(transform.position);
    }

    // ---------- BUCLE COMÚN ----------

    // Un solo Update para todos los enemigos: comprueba lo común y deja que cada hijo haga lo suyo
    private void Update()
    {
        if (_muerto) return;
        ActualizarFocus();   // 1º: ¿a quién persigo? (rellena o vacía _objetivo)
        Comportamiento();    // 2º: ahora que lo sé, cada hijo decide qué hace
    }

    // abstract → cada tipo de enemigo DEBE escribir qué hace en cada frame
    protected abstract void Comportamiento();

    // Decide si coge, mantiene o suelta un objetivo. Igual para TODOS los enemigos
    private void ActualizarFocus()
    {
        // CASO 1: ya tiene objetivo → comprobar si lo tiene que soltar
        if (_objetivo != null)
        {
            bool desactivado = !_objetivo.gameObject.activeInHierarchy;   // ¿El jugador se ha desactivado (por ejemplo, muerto)?
            bool muyLejos = Vector2.Distance(transform.position, _objetivo.position) > _distanciaPerderFocus; // ¿Se ha escapado?

            if (desactivado || muyLejos)
            {
                _objetivo = null;        // Lo suelta. El próximo frame volverá a buscar
            }
            return; // Si tiene objetivo NO busca otro: se queda con el que fijó (eso es el "focus")
        }

        // CASO 2: no tiene objetivo → buscar al jugador más cercano
        Transform cercano = BuscarJugadorCercano();

        if (cercano != null && Vector2.Distance(transform.position, cercano.position) <= _distanciaDeteccion)
        {
            _objetivo = cercano;
            Debug.Log(name + " hace focus a " + _objetivo.name);
        }
    }

    // ---------- MUERTE Y EDITOR ----------

    // Al morir: lo común (base) y después desaparece
    protected override void Morir()
    {
        base.Morir();
        Destroy(gameObject);
    }

    // virtual → los hijos lo AMPLÍAN con override + base.OnDrawGizmosSelected()
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;                                      // Amarillo = detección
        Gizmos.DrawWireSphere(transform.position, _distanciaDeteccion);
        Gizmos.color = Color.gray;                                        // Gris = perder focus
        Gizmos.DrawWireSphere(transform.position, _distanciaPerderFocus);
    }
}