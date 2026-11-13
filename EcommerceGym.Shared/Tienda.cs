namespace EcommerceGym.Shared;

public static class Tienda
{
    // Costo de envío fijo para el prototipo
    public const decimal CostoEnvio = 15000m;
    // true = solo pueden reseñar un producto los clientes que lo compraron (pedido ya confirmado)
    public static readonly bool ResenasSoloCompradores = true;
}