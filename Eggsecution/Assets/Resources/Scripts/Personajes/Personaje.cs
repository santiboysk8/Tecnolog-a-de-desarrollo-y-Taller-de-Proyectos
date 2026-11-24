using System;      // Para Action (eventos)
using UnityEngine;

// Los dos bandos del juego. Sirve para que nadie dañe a los de su mismo bando
public enum Bando
{
    Jugador,
    Enemigo
}

// abstract → no se puede poner "Personaje" en un objeto; solo sus hijos (PlayerMovement, EnemyMelee...)
public abstract class Personaje : MonoBehaviour
{
    [Header("VIDA")]
    [SerializeField] protected float _vidaMaxima = 100f; // protected → los hijos la ven; sale en el Inspector de cada hijo

    protected float _vidaActual;     // Vida que le queda ahora
    protected bool _muerto = false;  // Para no morir dos veces si le llegan dos golpes a la vez

    // Propiedades de solo lectura: otros scripts pueden LEER pero no cambiar
    public float VidaActual => _vidaActual;
    public float VidaMaxima => _vidaMaxima;
    public bool EstaMuerto => _muerto;

    public float MultiplicadorDeDanio { get; set; } = 1f; 

    // abstract → la base no lo sabe; cada hijo está OBLIGADO a decir su bando
    public abstract Bando Bando { get; }

    // Eventos: listas de funciones a las que avisar
    public event Action<float, float> OnVidaCambiada; // Envía (vidaActual, vidaMaxima)
    public event Action OnMuerte;

    // virtual → los hijos que necesiten Awake deben escribir "protected override void Awake()" y llamar a base.Awake()
    protected virtual void Awake()
    {
        _vidaActual = _vidaMaxima; // Empieza con la vida llena
    }
     public virtual void RecibirDanio(float cantidad)
    {
        if (_muerto) return;
        
        float danioReal = cantidad * MultiplicadorDeDanio;
        _vidaActual = Mathf.Max(_vidaActual - danioReal, 0f);
        Debug.Log(name + " recibe " + danioReal + " de daño. Vida: " + _vidaActual + "/" + _vidaMaxima); // NUEVO: para ver el daño real (combo de marcas, vida x2...)

        OnVidaCambiada?.Invoke(_vidaActual, _vidaMaxima);
        if (_vidaActual <= 0f)
        {
            Morir();
        }
    }

    public void Curar(float cantidad)
    {
        if (_muerto) return;

        _vidaActual = Mathf.Min(_vidaActual + cantidad, _vidaMaxima); // Suma sin pasarse del máximo
        OnVidaCambiada?.Invoke(_vidaActual, _vidaMaxima);
    }

    // virtual → cada hijo añade lo suyo (destruirse, reaparecer...), pero llamando antes a base.Morir()
    protected virtual void Morir()
    {
        _muerto = true;
        Debug.Log(name + " ha muerto");
        OnMuerte?.Invoke();
    }

    // Crea un ataque (golpe, bala...) y le dice de qué bando es.
    // La usan el jugador y los enemigos: así el bando nunca se configura a mano
    protected GameObject CrearAtaque(GameObject prefab, Vector3 posicion, Quaternion rotacion)
    {
        GameObject _object = Instantiate(prefab, posicion, rotacion);
        Ataque ataque = _object.GetComponent<Ataque>(); // CAMBIA: encuentra Bala o GolpeMelee (polimorfismo)
        if (ataque != null)
        {
            ataque.Configurar(Bando);
        }
        return _object;
    }
}
