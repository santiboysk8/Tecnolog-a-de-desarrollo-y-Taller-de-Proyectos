using UnityEngine;

// Aceituna: tirador básico. Su patrón (Recto) y su bala se eligen en el Inspector
public class Aceituna : EnemigoTirador
{
    // CAMBIA: ya no necesita Disparar() ni su propia bala (_balaAceituna):
    // todo lo hace EnemigoTirador con la ficha de patrón y su _proyectil
}
