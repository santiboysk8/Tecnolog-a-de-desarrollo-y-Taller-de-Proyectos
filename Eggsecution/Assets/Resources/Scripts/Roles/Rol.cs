using System;      // Para Action (evento)
using UnityEngine;

// abstract → no se puede poner "Rol" en un objeto; solo RolMago, RolBoxeador, etc.
public abstract class Rol : MonoBehaviour
{
    [SerializeField] protected float _recursoMaximo = 100f; // Maná, estamina, balas o furia según el hijo

    protected float _recurso;          // Cuánto le queda ahora
    protected PlayerMovement _jugador; // El jugador que lleva este rol (para lanzar cosas hacia donde apunta)

    public float Recurso => _recurso;              // Lectura desde fuera (para una futura barra de maná, por ejemplo)
    public float RecursoMaximo => _recursoMaximo;

    // abstract → cada rol DEBE decir cómo se llama su recurso (sale en los Debug.Log)
    public abstract string NombreRecurso { get; }

    // Aviso cuando cambia el recurso (igual que OnVidaCambiada). Sirve para pintar una barra de maná más adelante
    public event Action<float, float> OnRecursoCambiado;

    // virtual → los hijos pueden ampliarlo (el Boxeador empieza con furia 0, por ejemplo)
    protected virtual void Awake()
    {
        _jugador = GetComponent<PlayerMovement>(); // El rol está en el mismo objeto que PlayerMovement
        _recurso = _recursoMaximo;                 // Empieza lleno
    }

    // ---------- LO QUE CADA ROL TIENE QUE HACER ----------

    // abstract → OBLIGATORIO: todo rol tiene un ataque básico
    public abstract void Atacar();

    // virtual → OPCIONAL: si un rol no tiene habilidad, se usa este código por defecto
    public virtual void Habilidad1()
    {
        Debug.Log(GetType().Name + " no tiene Habilidad 1 todavía"); // GetType().Name = nombre de la clase real (p. ej., "RolMago")
    }

    public virtual void Habilidad2()
    {
        Debug.Log(GetType().Name + " no tiene Habilidad 2 todavía");
    }

    // ---------- HERRAMIENTAS PARA LOS HIJOS ----------

    // Intenta gastar recurso. Devuelve true si había suficiente (y lo gasta), false si no
    protected bool Gastar(float cantidad)
    {
        if (_recurso < cantidad)
        {
            Debug.Log("No hay suficiente " + NombreRecurso);
            return false;
        }
        CambiarRecurso(_recurso - cantidad);
        return true;
    }

    // Suma recurso sin pasarse del máximo (regenerar maná, ganar furia...)
    protected void Recuperar(float cantidad)
    {
        CambiarRecurso(Mathf.Min(_recurso + cantidad, _recursoMaximo));
    }

    // Único sitio donde se cambia _recurso → así siempre se avisa al evento
    protected void CambiarRecurso(float nuevoValor)
    {
        _recurso = nuevoValor;
        OnRecursoCambiado?.Invoke(_recurso, _recursoMaximo);
    }
}
