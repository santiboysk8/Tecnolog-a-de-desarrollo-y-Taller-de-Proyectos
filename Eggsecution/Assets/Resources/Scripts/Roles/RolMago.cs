using UnityEngine;

// MAGO: el ataque básico es gratis (con cadencia). Las habilidades gastan maná, que se regenera solo
public class RolMago : Rol // Hereda todo de Rol
{
    [Header("Ataque básico")]
    [SerializeField] private GameObject _fireball;               // S_Fireball
    [SerializeField] private float _tiempoEntreAtaques = 0.4f;   // Cadencia: segundos mínimos entre bola y bola

    [Header("Maná")]
    [SerializeField] private float _regeneracionPorSegundo = 5f;

    [Header("Habilidad 1: triple bola")]
    [SerializeField] private float _costeHabilidad1 = 30f;       // Maná que gasta
    [SerializeField] private int _bolasCirculo = 12;               // NUEVO: cuántas bolas salen
    [SerializeField] private float _aperturaBolas = 360f;        // NUEVO: ángulo total del abanico (30° → -15°, 0°, +15°)

    [Header("Habilidad 2: bola gigante")]
    [SerializeField] private float _costeHabilidad2 = 50f;       // Maná que gasta
    [SerializeField] private float _escalaGigante = 3f;          // NUEVO: cuántas veces más grande que una bola normal
    [SerializeField] private float _cooldownGigante = 4f;        // NUEVO: segundos de espera para volver a usarla


    private float _siguienteAtaque;   // Momento (Time.time) a partir del cual puede volver a atacar
    private float _siguienteGigante;  // NUEVO: momento a partir del cual puede volver a usar la Habilidad 2

    public override string NombreRecurso => "Maná"; // Obligatorio por ser abstract en Rol

    private void Update()
    {
        // Regenera poco a poco. * Time.deltaTime → 5 por SEGUNDO, no 5 por frame
        if (_recurso < _recursoMaximo)
        {
            Recuperar(_regeneracionPorSegundo * Time.deltaTime);
        }
    }

    // Ataque básico gratis: solo comprueba la cadencia
    public override void Atacar() // Obligatorio por ser abstract en Rol
    {
        if (Time.time < _siguienteAtaque) return;              // Aún no ha pasado el tiempo entre ataques → no hace nada

        _jugador.SpawnHaciaApuntado(_fireball);                // Lanza la bola gratis
        _siguienteAtaque = Time.time + _tiempoEntreAtaques;    // Apunta cuándo puede volver a atacar
    }

    // ---------- HABILIDAD 1: TRIPLE BOLA ----------
    // Misma idea que el abanico del Pistolero: repartir N bolas en un ángulo
    public override void Habilidad1()
    {
        if (!Gastar(_costeHabilidad1)) return;   // Sin maná suficiente, no hace nada (Gastar ya avisa por consola)

        float inicio = -_aperturaBolas / 2f;                                          // Primera bola: mitad de la apertura hacia un lado
        float paso = _bolasCirculo > 1 ? _aperturaBolas / (_bolasCirculo) : 0f;    // Separación entre bolas (3 bolas = 2 huecos)
        if (_bolasCirculo == 1) inicio = 0f;                                            // Con una sola bola, sale recta

        for (int i = 0; i < _bolasCirculo; i++)
        {
            _jugador.SpawnConDesvio(_fireball, inicio + paso * i); // -15°, 0°, +15°
        }
        Debug.Log("¡Triple bola! Maná: " + _recurso);
    }

    // ---------- HABILIDAD 2: BOLA GIGANTE ----------
    public override void Habilidad2()
    {
        // Primero el cooldown, para no gastar maná si aún está en espera
        if (Time.time < _siguienteGigante)
        {
            Debug.Log("Bola gigante en espera: " + (_siguienteGigante - Time.time).ToString("0.0") + " s"); // "0.0" = un decimal
            return;
        }

        if (!Gastar(_costeHabilidad2)) return;

        // SpawnHaciaApuntado DEVUELVE la bola creada → la guardamos para modificarla
        GameObject bola = _jugador.SpawnHaciaApuntado(_fireball);

        // localScale = tamaño del objeto. Multiplicarlo por 3 la hace 3 veces más grande en todos los ejes
        bola.transform.localScale *= _escalaGigante;

        _siguienteGigante = Time.time + _cooldownGigante; // Empieza la espera
        Debug.Log("¡Bola gigante! Maná: " + _recurso);
    }

}
