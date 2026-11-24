using UnityEngine;

public class MarcarAlImpactar : MonoBehaviour
{
    [SerializeField] private TipoMarca _tipo = TipoMarca.Sal;   // Qué marca pone esta bala

    private Ataque _ataque;   // La Bala de este mismo objeto (Bala ES un Ataque)

    private void Awake()
    {
        _ataque = GetComponent<Ataque>();
        if (_ataque == null)
        {
            Debug.LogWarning(name + ": MarcarAlImpactar necesita una Bala (o otro Ataque) en el mismo objeto");
            return;
        }
        _ataque.OnImpacto += PonerMarca;   // Sin paréntesis: no la llama, la APUNTA en la lista del evento
    }

    // Buena práctica (como en BarraVida): desuscribirse al destruirse
    private void OnDestroy()
    {
        if (_ataque != null)
        {
            _ataque.OnImpacto -= PonerMarca;
        }
    }

    // Ataque la llama justo después de hacer daño, pasando a quién ha dañado
    private void PonerMarca(Personaje personaje)
    {
        Marcar marcas = personaje.GetComponent<Marcar>();
        if (marcas == null)
        {
            // Por si se olvidó ponerlo en el prefab. Usará los valores por defecto del script
            marcas = personaje.gameObject.AddComponent<Marcar>();
        }
        marcas.AnadirMarca(_tipo);
    }
    
}
