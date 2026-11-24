using UnityEngine;

// abstract → "ficha" de patrón de disparo. Es un ScriptableObject: un ARCHIVO de datos, no un componente.
// REGLA: no guarda nada que cambie en la partida (lo comparten muchos enemigos)
public abstract class PatronDisparo : ScriptableObject
{
    // tirador        → quién dispara (le pediremos que cree las balas)
    // anguloObjetivo → grados desde el tirador hacia el jugador (0 = derecha, 90 = arriba)
    // rafaga         → número de disparo: 0, 1, 2... (para patrones que cambian en cada ráfaga, como la espiral)
    public abstract void Disparar(EnemigoTirador tirador, float anguloObjetivo, int rafaga);
}
