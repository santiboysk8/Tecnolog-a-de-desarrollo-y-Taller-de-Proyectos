using UnityEngine;

// BOXEADOR: los puñetazos no cuestan nada; al revés, GANAN furia.
// Con la furia al máximo puede usar una habilidad especial
public class RolBoxeador : Rol
{
    [SerializeField] private GameObject _punchCollider;     // C_Punch
    [SerializeField] private float _furiaPorGolpe = 5f;

    [Header("Habilidad 1: Wind Burst")]

    [SerializeField] private GameObject _windBurst;

    public override string NombreRecurso => "Furia";

    // override de Awake: el boxeador empieza con furia 0 en vez de llena
    protected override void Awake()
    {
        base.Awake();      // ¡Importante! Ejecuta el Awake de Rol (coge _jugador)...
        CambiarRecurso(0); // ...y después pone la furia a 0
    }

    public override void Atacar()
    {
        // CAMBIA: guardamos el puñetazo creado para poder destruirlo después
        _jugador.SpawnHaciaApuntado(_punchCollider); // Sin coste



        Recuperar(_furiaPorGolpe);
        Debug.Log("Furia: " + _recurso);
    }

    // Habilidad 1: solo con la furia llena; la gasta entera
    public override void Habilidad1()
    {
        if (Gastar(_recursoMaximo))
        {
            _jugador.SpawnHaciaApuntado(_windBurst);
            Debug.Log("¡FURIA DESATADA!"); // Aquí iría el efecto especial (más daño, más velocidad...)
        }
    }
}
