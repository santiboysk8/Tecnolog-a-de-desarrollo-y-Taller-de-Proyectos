using System.Collections.Generic;
using UnityEngine;


public class CameraFollow : MonoBehaviour
{
 public static CameraFollow Instance { get; private set; }

    [SerializeField] private float _smoothTime = 0.3f; // suavizado del movimiento 
    [SerializeField] private float _margin = 1f; // margen de distancia del borde de pantalla 
    [SerializeField] private float _extraMargin = 0.5f; // margen adicional para el sprite del jugador
    private readonly List<Transform> _players = new List<Transform>();
    private Camera _camera;
    private Vector3 _velocity;
    private float _zOffset;

    private void Awake()
    {
        Instance = this;
        _camera = GetComponent<Camera>();
        _zOffset = transform.position.z;
    }

    public void AddPlayer(Transform player)
    {
        if (!_players.Contains(player))
        {
            _players.Add(player);
        }
    }
    public void RemovePlayer(Transform player)
    {
        _players.Remove(player);
    }
    private void LateUpdate()
    {
        _players.RemoveAll(p => p == null); // Eliminar jugadores nulos de la lista
        if(_players.Count == 0) return;

        // 1 Seguir al jugador
        Transform leader = _players[0];
        Vector3 target = new Vector3(leader.position.x, leader.position.y, _zOffset);
        transform.position = Vector3.SmoothDamp(transform.position, target , ref _velocity, _smoothTime);

        // Calcula el borde visible de la camara
        CalcularMitadPantalla(out float halfW, out float halfH);
        Vector3 centro = transform.position;
        
        // Arrastra a los jugadores dentro de la camara 

        for(int i = 1; i < _players.Count; i++)
        {
            ArrastraDentro(_players[i], centro, halfW, halfH);
        }
 
    }

    private void CalcularMitadPantalla(out float halfW, out float halfH)
    {
        if (_camera.orthographic)
        {
            halfH = _camera.orthographicSize;
        }
        else
        {
            float distancia = Mathf.Abs(_zOffset);
            halfH = distancia * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        }
        halfW = halfH * _camera.aspect;
    }

    private float CalcularMargen(Transform player)
    {
        SpriteRenderer sr = player.GetComponentInChildren<SpriteRenderer>();
        if(sr == null || sr.sprite == null)
        {
            return _margin; // Si no hay SpriteRenderer, usar el margen predeterminado
        }

        Vector3 ext = Vector3.Scale(sr.sprite.bounds.extents, sr.transform.lossyScale);
        return Mathf.Max(Mathf.Abs(ext.x), Mathf.Abs(ext.y)) + _extraMargin;
    }

    private void ArrastraDentro(Transform player, Vector3 centro, float halfW, float halfH)
    {
        float margen = CalcularMargen(player);

        float minX = centro.x - halfW + margen;
        float maxX = centro.x + halfW - margen;
        float minY = centro.y - halfH + margen;
        float maxY = centro.y + halfH - margen;

        Vector3 pos = player.position;
        Vector3 dentro = new Vector3(Mathf.Clamp(pos.x, minX, maxX), Mathf.Clamp(pos.y, minY, maxY), pos.z);

        // Mueve al jugador dentro de los limites de la c�mara si est� fuera
        Vector3 delta = dentro - pos;
        if (delta.sqrMagnitude < 0.0001f) return; // ya esta dentro, no hace falta moverlo

        // CAMBIA: antes buscaba CharacterController. Con Rigidbody2D se coloca con rb.position
        // (no con transform.position, para no "engañar" a la física)
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.position = dentro;
        }
        else
        {
            player.position = dentro;
        }
    }
}
