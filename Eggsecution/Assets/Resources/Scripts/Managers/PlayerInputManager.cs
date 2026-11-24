using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    // NUEVO: un prefab por rol. MISMO ORDEN que el enum TipoRol: [0] Boxeador, [1] Mago, [2] Espadachin, [3] Pistolero
    [SerializeField] private GameObject[] _prefabsRoles;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private BarraVida[] _barrasVida; 
  

    private TipoRol RolJ1 =>  SeleccionPartida.RolJugador1;
    private TipoRol RolJ2 =>  SeleccionPartida.RolJugador2;

    private bool _playerGamepad = false;

    
    private void Start()
    {
        GameObject prefab = PrefabDe(RolJ1); // CAMBIA: antes PrefabDe(SeleccionPartida.RolJugador1)
        if (prefab == null) return;

        PlayerInput player;
        if (Mouse.current != null)
        {
            player = PlayerInput.Instantiate(prefab, controlScheme: "Teclado",
                                             pairWithDevices: new InputDevice[] { Keyboard.current, Mouse.current });
        }
        else
        {
            player = PlayerInput.Instantiate(prefab, controlScheme: "Teclado", pairWithDevice: Keyboard.current);
        }
        ColocarEnSpawn(player, 0);
        CameraFollow.Instance.AddPlayer(player.transform);
        AsignarBarra(player, 0); // NUEVO: conecta la barra de vida del J1
        Debug.Log("Jugador 1 (" + RolJ1 + ") con teclado + ratón"); // CAMBIA: muestra el rol realmente usado
    }

    // NUEVO: el Jugador 2 (si lo eligieron en el menú) entra al pulsar Select en su mando
    private void Update()
    {
        
        if (!SeleccionPartida.HayJugador2 || _playerGamepad) return;


        foreach (var gamePad in Gamepad.all)
        {
            if (gamePad.selectButton.wasPressedThisFrame)
            {
                GameObject prefab = PrefabDe(RolJ2); // CAMBIA: antes PrefabDe(SeleccionPartida.RolJugador2)
                if (prefab == null) return;

                var player = PlayerInput.Instantiate(prefab, controlScheme: "GamePad", pairWithDevice: gamePad);
                ColocarEnSpawn(player, 1);
                CameraFollow.Instance.AddPlayer(player.transform);
                AsignarBarra(player, 1); // NUEVO: conecta la barra de vida del J2
                _playerGamepad = true;
                Debug.Log("Jugador 2 (" + RolJ2 + ") con mando"); // CAMBIA: muestra el rol realmente usado
                break; // Ya ha entrado; no seguir mirando más mandos
            }
        }
    }

    // NUEVO: convierte el enum en un índice del array: (int)TipoRol.Mago = 1 → _prefabsRoles[1]
    private GameObject PrefabDe(TipoRol rol)
    {
        int indice = (int)rol;
        if (_prefabsRoles == null || indice >= _prefabsRoles.Length || _prefabsRoles[indice] == null)
        {
            Debug.LogError("Falta el prefab del rol " + rol + " en Prefabs Roles (posición " + indice + ")");
            return null;
        }
        return _prefabsRoles[indice];
    }

   
    // le da a la barra de vida número "indice" el jugador recién creado
    private void AsignarBarra(PlayerInput player, int indice)
    {
        // Si no hay barras puestas en el Inspector, no hace nada (el juego funciona igual, solo sin barra)
        if (_barrasVida == null || indice >= _barrasVida.Length || _barrasVida[indice] == null) return;

        Personaje personaje = player.GetComponent<Personaje>(); // Encuentra el PlayerMovement (ES un Personaje)
        _barrasVida[indice].AsignarPersonaje(personaje);
    }

    private void ColocarEnSpawn(PlayerInput player, int indice)
    {
        if (_spawnPoints == null || indice >= _spawnPoints.Length)
        {
            return;
        }
        player.transform.position = _spawnPoints[indice].position;
        Physics2D.SyncTransforms(); 
    }

}
