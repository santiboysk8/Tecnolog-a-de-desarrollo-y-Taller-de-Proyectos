using System.Collections;            // NUEVO: para IEnumerator (corrutinas)
using UnityEngine;
using UnityEngine.Serialization; // NUEVO: para FormerlySerializedAs

public enum ModoAtaque
{
    Perseguir,
    Huir,
    Patrullar
}
// Todos los tiradores: patrullan, se acercan hasta su rango y disparan con cooldown
public abstract class EnemigoTirador : Enemigo
{
    [Header("PATRULLA")]
    [SerializeField] protected Transform[] _puntosPatrulla;
    [SerializeField] protected float _velocidadPatrulla = 1f;
    [SerializeField] protected float _distanciaLlegada = 0.1f;

    [Header("ATAQUE A DISTANCIA")]
    [SerializeField] protected ModoAtaque _modo = ModoAtaque.Patrullar;   // CAMBIA: por defecto patrulla (tu diseño: los de distancia no persiguen)
    [SerializeField] protected PatronDisparo _patron;                     // NUEVO: la ficha de patrón (se arrastra en el Inspector)
    [FormerlySerializedAs("_balaAceituna")]                               // NUEVO: "antes me llamaba así" → S_Aceituna no pierde su bala
    [SerializeField] protected GameObject _proyectil;                     // NUEVO (subido desde Aceituna): la bala que dispara
    [SerializeField] protected float _speed = 1f;                 // Velocidad al perseguir
    [SerializeField] protected float _distanciaAtaque = 4f;
    [SerializeField] protected float _distanciaSegura = 3f;
    [SerializeField] protected float _tiempoEntreDisparos = 5f;
    [SerializeField] protected float _spawnDistance = 0.5f;
    // NUEVO: un "ataque" pueden ser varias ráfagas seguidas. 1 = como siempre (Aceituna, Sal)
    [SerializeField, Min(1)] protected int _rafagasPorAtaque = 1;
    [SerializeField] protected float _tiempoEntreRafagas = 0.15f;   // INVENTADO: segundos entre ráfaga y ráfaga

    private int _siguientePunto = 0;
    private float _siguienteDisparo;
    private int _rafagas = 0;   // NUEVO: cuántas ráfagas lleva disparadas (el patrón lo usa; NO se guarda en la ficha)
    private bool _atacando = false;   // NUEVO: true mientras dura la tanda de ráfagas (para no empezar otra encima)

    protected bool TienePatrulla => _puntosPatrulla != null && _puntosPatrulla.Length > 0;

    // Lo que antes era el Comportamiento de la Aceituna, ahora con patrulla
    protected override void Comportamiento()
    {
        if (_objetivo == null)
        {
            Patrullar();
            return;
        }
        float distancia = Vector2.Distance(transform.position, _objetivo.position);

        switch (_modo)
        {
            case ModoAtaque.Perseguir:
                MirarHacia(_objetivo.position.x);
                if (distancia > _distanciaAtaque)
                {
                    MoverHacia(_objetivo.position, _speed);
                    return;                     // Mientras se acerca no dispara
                }
                Detener();
                break;

            case ModoAtaque.Patrullar:
                Patrullar();
                break;

            case ModoAtaque.Huir:
                if (distancia < _distanciaSegura)
                    AlejarseDe(_objetivo.position, _speed);   // Demasiado cerca → retrocede
                else
                    Detener();                                // A buena distancia → quieto
                MirarHacia(_objetivo.position.x);             // Retrocede mirándolo, no de espaldas
                break;
        }

        IntentarDisparar();

        
    }
    // [ANTIGUO]
    // private void IntentarDisparar()
    // {
    //     if (Time.time < _siguienteDisparo) return;
    //     Disparar();
    //     _siguienteDisparo = Time.time + _tiempoEntreDisparos;
    // }
    // [FIN ANTIGUO]

    private void IntentarDisparar()
    {
        // CAMBIA: tampoco empieza si ya está en mitad de un ataque
        if (_atacando || Time.time < _siguienteDisparo) return;

        // El cooldown cuenta desde que EMPIEZA el ataque: 0.5 ataques/s = uno cada 2 s, dure lo que dure
        _siguienteDisparo = Time.time + _tiempoEntreDisparos;
        StartCoroutine(TandaDeRafagas());   // NUEVO
    }

    // NUEVO: dispara el patrón _rafagasPorAtaque veces, esperando _tiempoEntreRafagas entre una y otra.
    // Esto NO puede ir en el patrón: "llevo 3 de 8" es estado de la partida, y la ficha la comparten todos
    private IEnumerator TandaDeRafagas()
    {
        _atacando = true;

        for (int i = 0; i < _rafagasPorAtaque; i++)
        {
            if (_objetivo == null) break;   // El jugador se alejó o murió a mitad → corto (si no, Disparar daría error)

            Disparar();                     // Cada llamada suma 1 a _rafagas → la espiral va girando

            if (i < _rafagasPorAtaque - 1)  // Tras la ÚLTIMA no hace falta esperar
            {
                yield return new WaitForSeconds(_tiempoEntreRafagas);
            }
        }

        _atacando = false;
    }

    // NUEVO: si el enemigo se desactiva, Unity para sus corrutinas a medias.
    // Sin esto, _atacando se quedaría en true y al reactivarse no volvería a disparar nunca
    private void OnDisable()
    {
        _atacando = false;
    }
    // CAMBIA: antes era "protected abstract void Disparar();". Ahora tiene código:
    // le pide a su patrón que dispare. virtual → un hijo aún podría añadir algo (p. ej., un sonido)
    protected virtual void Disparar()
    {
        if (_patron == null || _proyectil == null)
        {
            Debug.LogWarning(name + ": falta asignar Patron o Proyectil en el Inspector");
            return;
        }

        // Ángulo hacia el jugador en grados (0 = derecha). Atan2 da radianes → Rad2Deg los pasa a grados
        Vector2 dir = _objetivo.position - transform.position;
        float anguloObjetivo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        _patron.Disparar(this, anguloObjetivo, _rafagas); // "this" = yo, el tirador que dispara
        _rafagas++;
    }

    // NUEVO: public → el PATRÓN (que es otra clase) puede llamarlo.
    // Crea UNA bala en el ángulo indicado, desde mi distancia de salida y con mi bando
    public GameObject CrearBala(float angulo, float multiplicadorVelocidad = 1f)
    {
        Vector3 direccion = Quaternion.Euler(0, 0, angulo) * Vector3.right;  // Vector que apunta en ese ángulo
        Vector3 posicion = transform.position + direccion * _spawnDistance * multiplicadorVelocidad;
        GameObject bala = CrearAtaque(_proyectil, posicion, Quaternion.Euler(0, 0, angulo - 90f));

        if (bala.TryGetComponent(out Bala componenteBala))
        {
            componenteBala.MultiplicadorVelocidad(multiplicadorVelocidad);
        }
        return bala;
    }

    protected void Patrullar()
    {
        if (!TienePatrulla) { Detener(); return; }

        Vector2 destino = _puntosPatrulla[_siguientePunto].position;
        if (Vector2.Distance(_rb.position, destino) <= _distanciaLlegada)
        {
            _siguientePunto = (_siguientePunto + 1) % _puntosPatrulla.Length;
            destino = _puntosPatrulla[_siguientePunto].position;
        }

        MoverHacia(destino, _velocidadPatrulla);
        MirarHacia(destino.x);
    }

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        if (_modo == ModoAtaque.Perseguir)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _distanciaAtaque);
        }

        if (TienePatrulla)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < _puntosPatrulla.Length; i++)
            {
                Transform a = _puntosPatrulla[i];
                Transform b = _puntosPatrulla[(i + 1) % _puntosPatrulla.Length];
                if (a != null && b != null) Gizmos.DrawLine(a.position, b.position);
            }
        }
        if (_modo == ModoAtaque.Huir)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, _distanciaSegura);
        }
    }
}
