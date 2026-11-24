using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoleSelectionManager : MonoBehaviour
{
    public static RoleSelectionManager Instance;

    [SerializeField] private Button _wizardButton;
    [SerializeField] private Button _shooterButton;
    [SerializeField] private Button _boxerButton;
    [SerializeField] private Button _sworderButton;
    [SerializeField] private TMP_Text _titulo;                 // NUEVO (opcional): texto "Jugador 1 elige rol"
    [SerializeField] private string _escenaNivel = "Zona-1";   // NUEVO: el nivel al que se va al terminar

    private int _jugadorEligiendo = 1; // NUEVO: 1 = elige el J1; 2 = elige el J2

    private void Awake()
    {
        if (RoleSelectionManager.Instance == null)
            RoleSelectionManager.Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        ActualizarTitulo();
    }

    // NUEVO: los cuatro botones, cada uno con su rol
    private void OnEnable()
    {
        _wizardButton.onClick.AddListener(ElegirMago);
        _shooterButton.onClick.AddListener(ElegirPistolero);
        _boxerButton.onClick.AddListener(ElegirBoxeador);
        _sworderButton.onClick.AddListener(ElegirEspadachin);
    }

    private void OnDisable()
    {
        _wizardButton.onClick.RemoveListener(ElegirMago);
        _shooterButton.onClick.RemoveListener(ElegirPistolero);
        _boxerButton.onClick.RemoveListener(ElegirBoxeador);
        _sworderButton.onClick.RemoveListener(ElegirEspadachin);
    }

    // Un método por botón: AddListener necesita una función sin parámetros
    private void ElegirMago()       { Elegir(TipoRol.Mago); }
    private void ElegirPistolero()  { Elegir(TipoRol.Pistolero); }
    private void ElegirBoxeador()   { Elegir(TipoRol.Boxeador); }
    private void ElegirEspadachin() { Elegir(TipoRol.Espadachin); }

    // Guarda el rol del jugador que está eligiendo y decide qué viene después
    private void Elegir(TipoRol rol)
    {
        if (_jugadorEligiendo == 1)
        {
            SeleccionPartida.RolJugador1 = rol;
            Debug.Log("Jugador 1: " + rol);

            if (SeleccionPartida.HayJugador2) // Multijugador → ahora le toca al J2 en la misma pantalla
            {
                _jugadorEligiendo = 2;
                ActualizarTitulo();
                return;                       // No carga el nivel todavía
            }
        }
        else
        {
            SeleccionPartida.RolJugador2 = rol;
            Debug.Log("Jugador 2: " + rol);
        }

        Time.timeScale = 1;
        SceneManager.LoadScene(_escenaNivel); // Todos han elegido → al nivel
    }

    private void ActualizarTitulo()
    {
        if (_titulo != null) // Es opcional: si no hay texto asignado, no hace nada
        {
            _titulo.text = "Jugador " + _jugadorEligiendo + " elige rol";
        }
    }

    // [ANTIGUO] Versión anterior: solo el botón Wizard, no guardaba el rol y cargaba por número. Borrar cuando todo funcione
    //private void OnPlay()
    //{
    //    Time.timeScale = 1;
    //    SceneManager.LoadScene(3);
    //}
    // [FIN ANTIGUO]
}
