using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Personaje // CAMBIA: antes MonoBehaviour. Personaje ya hereda de MonoBehaviour
{
   

    // NUEVO: obligatorio porque Bando es abstract en Personaje
    public override Bando Bando => Bando.Jugador;

    public static readonly List<Transform> Jugadores = new List<Transform>();
    [Header("MOVIMIENTO")]
    //Movimiento común
    [SerializeField] float _speed = 5f;
    [SerializeField] float _dashCooldown= 1.5f;
    [SerializeField] float _dashDuration = 0.2f;
    [SerializeField] float _dashDistance = 9f;
    [SerializeField] private Camera _camera;
    private Vector3 _objective;

    [Header("Camara")]
    [SerializeField] private Transform _visual;
    [SerializeField] private float _spawnDistance = 1.25f;

    [Header("Apuntado")]
    private Vector2 _aimDirection = Vector2.right;
    private PlayerInput _playerInput;

    private Rol _rol; // NUEVO: el rol que tenga este prefab (RolMago, RolBoxeador...). Se busca en Awake

    



    private bool _dashActive = false;
    private bool _isDashing = false;

    private Rigidbody2D _rb;                       // CAMBIA: antes CharacterController _controller
    private Vector2 _moveInput;                    // CAMBIA: Vector2 (antes Vector3): en 2D solo hay X e Y
    private Vector2 _lastMoveDirection = Vector2.right;
    private Vector2 _dashDirection;                // NUEVO: dirección del dash en curso (la usa FixedUpdate)

    // CAMBIA: antes "private void Awake()". Ahora sobrescribe el Awake de Personaje
    protected override void Awake()
    {
        base.Awake(); // ¡IMPRESCINDIBLE! Pone la vida al máximo (Awake de Personaje)

        _rb = GetComponent<Rigidbody2D>();         // CAMBIA: antes GetComponent<CharacterController>()
        _rb.gravityScale = 0f;                     // NUEVO: vista cenital → sin gravedad (si no, el huevo "cae" hacia abajo)
        _rb.freezeRotation = true;                 // NUEVO: que los choques no lo hagan girar como una peonza

        _playerInput = GetComponent<PlayerInput>();
        _rol = GetComponent<Rol>(); // NUEVO: encuentra CUALQUIER hijo de Rol (RolMago, RolBoxeador...). Eso es polimorfismo
        if (_rol == null)
        {
            Debug.LogWarning(name + " no tiene ningún Rol: no podrá atacar"); // Aviso si se te olvida poner el componente
        }

        if(_camera == null)
        {
            _camera = Camera.main;
        }
    }

    private  void OnEnable()
    {
        Jugadores.Add(transform);
        
    }

    public static Transform JugadorMasCercano(Vector3 posicion)
    {
        Transform cercano = null;
        float mejorDistancia = float.MaxValue;

        foreach(Transform jugador in Jugadores)
        {
            float distancia = Vector2.Distance(posicion, jugador.position);
            if(distancia < mejorDistancia)
            {
                mejorDistancia = distancia;
                cercano = jugador;
            }
        }
        return cercano;
    }
    private void OnDisable()
    {
        Jugadores.Remove(transform);
    }

    // NUEVO: qué pasa cuando el jugador muere
    protected override void Morir()
    {
        base.Morir();        // Lo común: marcar muerto y avisar (la barra se vacía)
        Destroy(gameObject); // OnDisable lo saca de Jugadores y CameraFollow limpia los destruidos
    }
    public void Aim (InputAction.CallbackContext _context) //Apuntado del personaje con el Mouse y Joystick
    {
        Vector2 stick = _context.ReadValue<Vector2>();
        if (stick.sqrMagnitude > 0.04f)
        {
            _aimDirection = stick.normalized;
  
        }
    }

    public void ActualizarApuntado() //Apuntado del mouse
    {
        bool usaRaton = _playerInput.currentControlScheme == "Teclado";
        if(usaRaton && Mouse.current != null)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            mousePos.z = Mathf.Abs(_camera.transform.position.z);
            _objective = _camera.ScreenToWorldPoint(mousePos);

            Vector2 dir = _objective - transform.position;
            if(dir.sqrMagnitude > 0.01f)
            {
                _aimDirection = dir.normalized;
            }
        }

        float angulo = Mathf.Atan2(_aimDirection.y, _aimDirection.x) * Mathf.Rad2Deg - 90f;
        _visual.rotation = Quaternion.Euler(0, 0, angulo);
    }

    // NUEVO: crea un prefab girado "grados" respecto a donde apunta el jugador.
    // 0 = recto, +15 = 15° a la izquierda, -15 = 15° a la derecha (en Unity 2D los grados positivos giran en sentido antihorario)
    public GameObject SpawnConDesvio(GameObject prefab, float grados)
    {
        // Gira el vector de apuntado. Quaternion * Vector3 = el vector rotado
        Vector3 dir = Quaternion.Euler(0, 0, grados) * new Vector3(_aimDirection.x, _aimDirection.y, 0);

        Vector3 pos = transform.position + dir * _spawnDistance;          // Sale desde el borde del huevo en esa dirección
        float angulo = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;   // Igual que en ActualizarApuntado: el eje Y del prefab mira hacia dir
        return CrearAtaque(prefab, pos, Quaternion.Euler(0, 0, angulo));  // CAMBIA: antes Instantiate. CrearAtaque (de Personaje) además le pone el bando Jugador
    }

    // Ahora es solo "disparar con desvío 0". Así solo hay UN sitio que crea ataques (SpawnConDesvio)
    public GameObject SpawnHaciaApuntado(GameObject prefab)
    {
        return SpawnConDesvio(prefab, 0f);
    }

    public void Move(InputAction.CallbackContext _context)
    {
        _moveInput = _context.ReadValue<Vector2>();
    }

    // NUEVO: el ataque lo hace el rol. PlayerMovement no sabe si es mago o pistolero: cada uno hace "su" Atacar
    public void Attack(InputAction.CallbackContext _context)
    {
        if (_context.performed && _rol != null)
        {
            _rol.Atacar();
        }
    }

    // NUEVO: igual con las habilidades. Si el rol no tiene habilidad, sale el mensaje por defecto de Rol
    public void Habilidad1(InputAction.CallbackContext _context)
    {
        if (_context.performed && _rol != null)
        {
            _rol.Habilidad1();
        }
    }

    public void Habilidad2(InputAction.CallbackContext _context)
    {
        if (_context.performed && _rol != null)
        {
            _rol.Habilidad2();
        }
    }

    public void Dash(InputAction.CallbackContext _context)
    {
        if (_context.performed && !_dashActive)
        {
           StartCoroutine(IniciarDash(_lastMoveDirection));
        }
    }


    // CAMBIA: el dash ya no mueve nada; solo ACTIVA el modo dash durante _dashDuration segundos.
    // Quien mueve es FixedUpdate, que mira _isDashing
    IEnumerator IniciarDash(Vector2 direction)
    {
        _dashActive = true;
        _isDashing = true;
        _dashDirection = direction;

        yield return new WaitForSeconds(_dashDuration); // Mientras tanto, FixedUpdate usa la velocidad de dash

        _isDashing = false;
        yield return new WaitForSeconds(_dashCooldown);
        _dashActive = false;
    }

    // Update: solo LEER y DECIDIR (apuntado, última dirección). Ya no mueve
    private void Update()
    {
        ActualizarApuntado();

        if (_moveInput.sqrMagnitude > 0.01f)
        {
            _lastMoveDirection = _moveInput.normalized;
        }
    }

    // NUEVO: FixedUpdate: APLICAR la física. Unity lo llama a ritmo fijo (50 veces/s), sincronizado con el motor de física
    private void FixedUpdate()
    {
        if (_isDashing)
        {
            float dashSpeed = _dashDistance / Mathf.Max(_dashDuration, 0.01f); // Velocidad = distancia / tiempo (evita dividir entre 0)
            _rb.linearVelocity = _dashDirection * dashSpeed; // CAMBIA: antes _controller.Move. Ahora le damos velocidad al Rigidbody2D
        }
        else
        {
            _rb.linearVelocity = _moveInput * _speed;
        }
    }
}

