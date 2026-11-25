using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private Button _playButton;
    [SerializeField] private Button _optionButton;
    [SerializeField] private Button _creditsButton;
    [SerializeField] private Button _exitButton;
    [SerializeField] private string _escenaSiguiente = "PlayerSelection"; // NUEVO: por NOMBRE, no por número
    [SerializeField] private string _escenaOption = "Option";
    [SerializeField] private string _escenaCredits = "Credits";

    private void Awake()
    {
        if (MenuManager.Instance == null)
            MenuManager.Instance = this;
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlay);
        _optionButton.onClick.AddListener(OnOption);
        _creditsButton.onClick.AddListener(OnCredits);
        _exitButton.onClick.AddListener(OnExit); // NUEVO: el botón Exit ya hace algo
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlay);
        _optionButton.onClick.RemoveListener(OnOption);
        _creditsButton.onClick.RemoveListener(OnCredits);
        _exitButton.onClick.RemoveListener(OnExit);
    }

    private void OnPlay()
    {
        Time.timeScale = 1;
        RunData.CurrentZone = 0; 
        SceneManager.LoadScene(_escenaSiguiente);
    }

    private void OnOption()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(_escenaOption);
    }

    private void OnCredits()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(_escenaCredits);
    }

    // NUEVO: cierra el juego (en el editor no hace nada, por eso el Debug.Log)
    private void OnExit()
    {
        Debug.Log("Salir del juego");
        Application.Quit();
    }

    // [ANTIGUO] Versión anterior de OnPlay (cargaba por número). Borrar cuando todo funcione
    //private void OnPlay()
    //{
    //    Time.timeScale = 1;
    //    SceneManager.LoadScene(1);
    //}
    // [FIN ANTIGUO]
}
