using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelectionManager : MonoBehaviour
{
    public static PlayerSelectionManager Instance;

    [SerializeField] private Button _singlePlayerButton;
    [SerializeField] private Button _multiPlayerButton;
    [SerializeField] private string _escenaSiguiente = "RoleSelection"; // NUEVO: por NOMBRE, no por número

    private void Awake()
    {
        if (PlayerSelectionManager.Instance == null)
            PlayerSelectionManager.Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        _singlePlayerButton.onClick.AddListener(OnSinglePlayer); // NUEVO: antes Single-player no hacía nada
        _multiPlayerButton.onClick.AddListener(OnMultiPlayer);
    }

    private void OnDisable()
    {
        _singlePlayerButton.onClick.RemoveListener(OnSinglePlayer);
        _multiPlayerButton.onClick.RemoveListener(OnMultiPlayer);
    }

    // NUEVO: guarda en SeleccionPartida cuántos jugadores hay
    private void OnSinglePlayer()
    {
        SeleccionPartida.HayJugador2 = false;
        IrASeleccionDeRol();
    }

    private void OnMultiPlayer()
    {
        SeleccionPartida.HayJugador2 = true; // El J2 podrá unirse con Select en el nivel
        IrASeleccionDeRol();
    }

    private void IrASeleccionDeRol()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(_escenaSiguiente);
    }

    // [ANTIGUO] Versión anterior: solo el botón multijugador, cargaba por número. Borrar cuando todo funcione
    //private void OnPlay()
    //{
    //    Time.timeScale = 1;
    //    SceneManager.LoadScene(2);
    //}
    // [FIN ANTIGUO]
}
