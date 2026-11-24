using UnityEngine;

[CreateAssetMenu(fileName = "PatronForma", menuName = "Patrones/PatronForma")]
public class PatronFOrma : PatronDisparo
{

    // [Min(x)] → el Inspector no deja poner menos de x. Con 1 punta no hay forma
    [SerializeField, Min(2)] private int _puntas = 5;                      // Inventado: estrella de 5
    [SerializeField, Min(1)] private int _numeroBalas = 30;                // Inventado. Se redondea al múltiplo de (2 × puntas)
    [SerializeField, Min(0.05f)] private float _velocidadMinima = 0.4f;    // Multiplicador en los VALLES (1 = velocidad de la bala)
    [SerializeField, Min(0.05f)] private float _velocidadMaxima = 1f;      // Multiplicador en las PUNTAS
    [SerializeField] private bool _apuntarAlJugador = true;                // Inventado: true → una punta mira al jugador

    // Lados de la forma: punta→valle→punta... Una estrella de 5 tiene 10 lados
    private int Lados => _puntas * 2;

    // Balas en cada lado. RoundToInt redondea al entero más cercano (32/10 = 3.2 → 3). Como mínimo 1
    private int BalasPorLado => Mathf.Max(1, Mathf.RoundToInt((float)_numeroBalas / Lados));

    public override void Disparar(EnemigoTirador tirador, float anguloObjetivo, int rafaga)
    {
        float anguloBase = _apuntarAlJugador ? anguloObjetivo : 0f;

        for (int lado = 0; lado < Lados; lado++)
        {
            // Los dos vértices de este lado: uno es punta y el otro valle (o al revés)
            Vector2 a = Vertice(lado);
            Vector2 b = Vertice((lado + 1) % Lados);   // % → el último lado se une con el primero

            for (int j = 0; j < BalasPorLado; j++)
            {
                // t va de 0 a casi 1. NO llega a 1: ese punto es el vértice b, y ya lo pone el lado siguiente
                float t = (float)j / BalasPorLado;
                Vector2 punto = Vector2.Lerp(a, b, t);   // Lerp = el punto que está a un "t" del camino de a a b

                float angulo = Mathf.Atan2(punto.y, punto.x) * Mathf.Rad2Deg + anguloBase; // Hacia dónde va
                float velocidad = punto.magnitude;                                         // Distancia al centro = velocidad

                tirador.CrearBala(angulo, velocidad);
            }
        }
    }

    // Vértice número i. Los pares son PUNTAS (velocidad máxima) y los impares VALLES (velocidad mínima).
    // Están repartidos cada 180/puntas grados: estrella de 5 → 0° punta, 36° valle, 72° punta...
    private Vector2 Vertice(int i)
    {
        float grados = i * 180f / _puntas;
        float radio = (i % 2 == 0) ? _velocidadMaxima : _velocidadMinima;
        Vector2 direccion = Quaternion.Euler(0, 0, grados) * Vector2.right;
        return direccion * radio;
    }

    // OnValidate: Unity lo llama EN EL EDITOR cada vez que cambias un valor en el Inspector.
    // Avisa si el número de balas no cuadra, sin llenar la consola durante la partida
    private void OnValidate()
    {
        int reales = BalasPorLado * Lados;
        if (reales != _numeroBalas)
        {
            Debug.LogWarning(name + ": con " + _puntas + " puntas salen " + reales + " balas, no " + _numeroBalas);
        }
    }
}

