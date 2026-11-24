using UnityEngine;

// Los dos tipos de marca. Van en este archivo porque solo los usa el sistema de marcas
// (igual que Bando está en Personaje.cs)
public enum TipoMarca
{
    Sal,
    Pimienta
}

// Va en el jugador. Recuerda qué marcas tiene y hasta cuándo,
// ajusta el daño que recibe y enciende/apaga sus efectos visuales
public class Marcar : MonoBehaviour
{
    [Header("DURACIÓN Y DAÑO")]
    [SerializeField] private float _duracionNormal = 3f;        // una marca sola dura 3 s
    [SerializeField] private float _duracionCombo = 5f;         //  con las dos, ambas duran 5 s
    [SerializeField] private float _multiplicadorCombo = 1.5f;  // INVENTADO: el GDD no dice cuánto más daño

    [Header("EFECTOS VISUALES (opcionales)")]
    // NUEVO: hijos del jugador. Este script solo los ENCIENDE y APAGA; lo que contengan
    // (partículas, sprite con Animator...) lo decide arte. Si están vacíos, no pasa nada
    [SerializeField] private GameObject _efectoSal;
    [SerializeField] private GameObject _efectoPimienta;
    [SerializeField] private GameObject _efectoCombo;

    // Momento (Time.time) en que se acaba cada marca. Empiezan en 0 → "ya acabada"
    private float _finSal;
    private float _finPimienta;

    // CAMBIA: antes solo _comboAnterior. Ahora recuerda las tres cosas para actuar SOLO cuando alguna cambia
    private bool _salAnterior = false;
    private bool _pimientaAnterior = false;
    private bool _comboAnterior = false;

    private Personaje _personaje;          // El jugador (PlayerMovement ES un Personaje)

    // Propiedades calculadas: ¿sigue activa? = ¿aún no ha llegado su momento de fin?
    public bool TieneSal => Time.time < _finSal;
    public bool TienePimienta => Time.time < _finPimienta;
    public bool Combo => TieneSal && TienePimienta;

    private void Awake()
    {
        _personaje = GetComponent<Personaje>();

        // NUEVO: empezar limpio aunque en el prefab se hayan dejado los efectos encendidos
        ActivarEfecto(_efectoSal, false);
        ActivarEfecto(_efectoPimienta, false);
        ActivarEfecto(_efectoCombo, false);
    }

    // La llama MarcarAlImpactar cuando una bala de Sal o Pimienta daña al jugador
    public void AnadirMarca(TipoMarca tipo)
    {
        // ¿Está activa la OTRA marca? (si me pega Sal, miro Pimienta, y al revés)
        bool otraActiva = (tipo == TipoMarca.Sal) ? TienePimienta : TieneSal;

        if (otraActiva)
        {
            // Combo: las DOS pasan a durar 5 s desde ahora
            float fin = Time.time + _duracionCombo;
            _finSal = fin;
            _finPimienta = fin;
        }
        else if (tipo == TipoMarca.Sal)
        {
            _finSal = Time.time + _duracionNormal;
        }
        else
        {
            _finPimienta = Time.time + _duracionNormal;
        }

        Debug.Log(name + " marcado con " + tipo + (otraActiva ? " (combo, 5 s)" : " (3 s)"));
        ActualizarEstado(); // CAMBIA: antes ActualizarMultiplicador()
    }

    // Cada frame comprueba si alguna marca ha caducado (nadie "avisa" cuando pasa el tiempo)
    private void Update()
    {
        ActualizarEstado(); // CAMBIA: antes ActualizarMultiplicador()
    }

    // [ANTIGUO]
    // private void ActualizarMultiplicador()
    // {
    //     bool combo = Combo;
    //     if (combo == _comboAnterior) return;
    //     _comboAnterior = combo;
    //
    //     _personaje.MultiplicadorDeDanio = combo ? _multiplicadorCombo : 1f;
    //     Debug.Log(name + (combo ? " ¡COMBO Sal + Pimienta! Daño x" + _multiplicadorCombo : ": fin del combo"));
    // }
    // [FIN ANTIGUO]

    // CAMBIA: antes ActualizarMultiplicador. Ahora además enciende/apaga los efectos
    private void ActualizarEstado()
    {
        bool sal = TieneSal;
        bool pimienta = TienePimienta;
        bool combo = sal && pimienta;

        // Nada ha cambiado desde el último frame → no toco nada (lo normal casi siempre)
        if (sal == _salAnterior && pimienta == _pimientaAnterior && combo == _comboAnterior) return;

        // El multiplicador solo cambia al empezar o acabar el combo
        if (combo != _comboAnterior && _personaje != null)
        {
            _personaje.MultiplicadorDeDanio = combo ? _multiplicadorCombo : 1f;
            Debug.Log(name + (combo ? " ¡COMBO Sal + Pimienta! Daño x" + _multiplicadorCombo : ": fin del combo"));
        }

        // NUEVO: cada efecto, encendido mientras su marca esté activa
        ActivarEfecto(_efectoSal, sal);
        ActivarEfecto(_efectoPimienta, pimienta);
        ActivarEfecto(_efectoCombo, combo);

        _salAnterior = sal;
        _pimientaAnterior = pimienta;
        _comboAnterior = combo;
    }

    // NUEVO: enciende o apaga un efecto, si existe. Al ENCENDER un objeto, sus partículas
    // (Play On Awake) o su Animator empiezan solos
    private void ActivarEfecto(GameObject efecto, bool activo)
    {
        if (efecto != null)
        {
            efecto.SetActive(activo);
        }
    }
}
