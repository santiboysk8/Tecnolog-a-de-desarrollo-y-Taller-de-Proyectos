using UnityEngine; // Da acceso a MonoBehaviour, Vector2, Time, Debug, etc.

// Enemigo cuerpo a cuerpo: cuando un jugador se acerca le hace focus (eso lo hace Enemigo),
// lo persigue y le ataca cuando está a rango.
public class EnemyMelee : Enemigo // Hereda de Enemigo (y por tanto de Personaje: tiene vida)
{
    // ---------- VARIABLES DEL INSPECTOR ----------
    // La detección (_distanciaDeteccion, _distanciaPerderFocus, _objetivo) ya NO está aquí: la hereda de Enemigo

    [SerializeField] private float _speed = 2f;                // Velocidad a la que persigue (unidades por segundo)
    [SerializeField] private float _distanciaAtaque = 1.2f;    // A esta distancia deja de andar y empieza a atacar
    [SerializeField] private float _tiempoEntreAtaques = 1f;   // Segundos de espera entre un golpe y el siguiente (cooldown)

    [SerializeField] private GameObject _ataquePrefab;         // Prefab que aparece al golpear (puedes usar C_Punch o C_Sword). Si está vacío, solo hace Debug.Log
    [SerializeField] private float _spawnDistance = 0.8f;      // A qué distancia del enemigo aparece el golpe


    // ---------- VARIABLES INTERNAS ----------
    private float _siguienteAtaque;   // Momento (en segundos de juego) a partir del cual puede volver a atacar

    // Implementación obligatoria del abstract Comportamiento() de Enemigo.
    // Cuando Enemigo llama a esto, ActualizarFocus() YA se ha ejecutado: _objetivo está al día
    protected override void Comportamiento()
    {
        if (_objetivo == null)
        {
            Detener();   // Sin objetivo se queda quieto (con física hay que poner la velocidad a 0 explícitamente)
            return;
        }

        // Distancia 2D entre el enemigo y su objetivo
        float distancia = Vector2.Distance(transform.position, _objetivo.position);

        if (distancia > _distanciaAtaque) // ¿Está lejos para golpear?
        {
            MoverHacia(_objetivo.position, _speed); // Le da velocidad al Rigidbody2D (heredado de Enemigo); la física lo para contra las paredes
        }
        else
        {
            Detener();                              // Al llegar se para para atacar
            if (Time.time >= _siguienteAtaque)      // ¿Ha pasado ya el cooldown?
            {
                Atacar();                                           // Golpea
                _siguienteAtaque = Time.time + _tiempoEntreAtaques; // Y apunta cuándo puede volver a golpear
            }
        }
    }
    protected virtual GameObject Atacar()
    {
        return CrearHacia(_ataquePrefab, _objetivo, _spawnDistance);
    }

    // CAMBIA: override → amplía el del padre. base. dibuja amarillo y gris; aquí solo se añade el rojo
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();                                      // Amarillo + gris (de Enemigo)
        Gizmos.color = Color.red;                                         // Rojo = ataque (solo el melee lo tiene)
        Gizmos.DrawWireSphere(transform.position, _distanciaAtaque);
    }
}
