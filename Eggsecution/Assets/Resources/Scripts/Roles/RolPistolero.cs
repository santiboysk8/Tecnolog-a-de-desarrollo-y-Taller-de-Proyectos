using UnityEngine;

// PISTOLERO: su recurso es la munición. Todo gasta balas; con 0 no puede hacer nada
public class RolPistolero : Rol
{
    // _recursoMaximo (de Rol) = munición máxima. Ponlo en el Inspector (p. ej., 30)

    [Header("Disparo normal")]
    [SerializeField] private GameObject _bala;                    // S_Bala
    [SerializeField] private float _tiempoEntreDisparos = 0.15f;  // Espera baja: dispara rápido

    [Header("Habilidad 1: acordeón (abanico)")]
    [SerializeField] private int _balasAbanico = 5;               // Cuántas balas salen
    [SerializeField] private float _aperturaAbanico = 60f;        // Ángulo total del abanico en grados

    [Header("Habilidad 2: explosión desde el huevo")]
    [SerializeField] private int _balasExplosion = 8;             // Cuántas balas salen alrededor (8 = cada 45°)
    [SerializeField] private float _cooldownExplosion = 5f;       // Segundos de espera para volver a usarla

    private float _siguienteDisparo;    // Momento a partir del cual puede volver a disparar
    private float _siguienteExplosion;  // Momento a partir del cual puede volver a usar la Habilidad 2

   
    public override string NombreRecurso => "Balas";

    // ---------- DISPARO NORMAL ----------
    public override void Atacar()
    {
        if (Time.time < _siguienteDisparo) return;   // Respeta la cadencia

        if (Gastar(1))                               // 1 bala; si no hay, Gastar avisa y devuelve false
        {
            _jugador.SpawnHaciaApuntado(_bala);
            _siguienteDisparo = Time.time + _tiempoEntreDisparos;
        }
    }

    // ---------- HABILIDAD 1: ACORDEÓN ----------
    public override void Habilidad1()
    {
        if (!Gastar(_balasAbanico)) return;          // Gasta una bala por cada bala del abanico. Si no hay suficientes, nada

        // Ángulo de la primera bala: la mitad de la apertura hacia un lado (60° → empieza en -30°)
        float inicio = -_aperturaAbanico / 2f;

        // Separación entre balas. Con 5 balas y 60° → 60 / 4 = 15°.
        // Se divide entre (balas - 1) porque hay un hueco MENOS que balas (5 balas = 4 huecos).
        // Si solo hay 1 bala, el paso es 0 (y sale recta) para no dividir entre 0
        float paso = _balasAbanico > 1 ? _aperturaAbanico / (_balasAbanico - 1) : 0f;
        if (_balasAbanico == 1) inicio = 0f;

        for (int i = 0; i < _balasAbanico; i++)      // Una vuelta por cada bala
        {
            float grados = inicio + paso * i;        // -30, -15, 0, 15, 30
            _jugador.SpawnConDesvio(_bala, grados);
        }
        Debug.Log("¡Acordeón! Balas: " + _recurso);
    }

    // ---------- HABILIDAD 2: EXPLOSIÓN DESDE EL HUEVO ----------
    public override void Habilidad2()
    {
        // Primero el cooldown: si aún no ha pasado el tiempo, avisa cuánto falta
        if (Time.time < _siguienteExplosion)
        {
            float falta = _siguienteExplosion - Time.time;
            Debug.Log("Explosión en espera: " + falta.ToString("0.0") + " s"); // "0.0" = con un decimal
            return;
        }

        if (!Gastar(_balasExplosion)) return;        // Sin munición suficiente, ni siquiera empieza el cooldown

        float paso = 360f / _balasExplosion;         // Vuelta completa repartida: 8 balas → cada 45°

        for (int i = 0; i < _balasExplosion; i++)
        {
            _jugador.SpawnConDesvio(_bala, paso * i); // 0, 45, 90, 135... alrededor del huevo
        }

        _siguienteExplosion = Time.time + _cooldownExplosion; // Empieza la espera
        Debug.Log("¡Explosión! Balas: " + _recurso);
    }

    // Para que algo externo (caja de munición, por ejemplo) le dé balas. Recuperar no deja pasar del máximo
    public void AnadirBalas(int cantidad)
    {
        Recuperar(cantidad);
        Debug.Log("Munición recogida. Balas: " + _recurso);
    }

  
}
