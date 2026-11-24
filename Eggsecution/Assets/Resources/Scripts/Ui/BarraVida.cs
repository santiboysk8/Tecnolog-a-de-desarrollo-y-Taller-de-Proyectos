using UnityEngine;
using UnityEngine.UI; // Para Image

// Va en la barra de la UI. Muestra la vida de UN personaje
public class BarraVida : MonoBehaviour
{
    [SerializeField] private Image _relleno;                  // Image con Image Type = Filled
    [SerializeField] private bool _ocultarSinJugador = true;  // Esconder la barra hasta que aparezca su jugador

    private Personaje _personaje; // Al que está mostrando

    private void Awake()
    {
        if (_ocultarSinJugador)
        {
            gameObject.SetActive(false);
        }
    }

    // La llama PlayerInputManager justo después de crear al jugador
    public void AsignarPersonaje(Personaje personaje)
    {
        Desuscribirse(); // Por si ya mostraba a otro

        _personaje = personaje;
        if (_personaje == null) return;

        // Se suscribe SIN paréntesis: no llama a Actualizar, la apunta en la lista del evento
        _personaje.OnVidaCambiada += Actualizar;
        _personaje.OnMuerte += AlMorir;

        gameObject.SetActive(true);
        Actualizar(_personaje.VidaActual, _personaje.VidaMaxima); // Pintarla ya con la vida actual
    }

    private void Actualizar(float actual, float maxima)
    {
        _relleno.fillAmount = actual / maxima; // 0 = vacía, 1 = llena. Ejemplo: 70/100 = 0.7
    }

    private void AlMorir()
    {
        _relleno.fillAmount = 0f;
    }

    private void Desuscribirse()
    {
        if (_personaje != null)
        {
            _personaje.OnVidaCambiada -= Actualizar;
            _personaje.OnMuerte -= AlMorir;
        }
    }

    // Buena práctica: desuscribirse siempre al destruirse
    private void OnDestroy()
    {
        Desuscribirse();
    }
}
