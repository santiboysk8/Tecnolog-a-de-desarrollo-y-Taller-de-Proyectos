// Lista de roles. El ORDEN importa: se usa como índice del array _prefabsRoles de PlayerInputManager
public enum TipoRol
{
    Boxeador,   // 0
    Mago,       // 1
    Espadachin, // 2
    Pistolero   // 3
}

// static class → no se puede instanciar ni poner en un objeto; sus datos viven mientras el juego esté abierto.
// Es el "puente" entre la escena del menú y la del nivel
public static class SeleccionPartida
{
    public static TipoRol RolJugador1 = TipoRol.Boxeador; // Valor por defecto si no eliges nada (o si pruebas el nivel sin pasar por el menú)
    public static bool HayJugador2 = false;              // ¿Se ha unido un segundo jugador? Lo pone a true el botón Multi-player de PlayerSelection
    public static TipoRol RolJugador2 = TipoRol.Mago;
}
