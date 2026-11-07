using EcommerceGym.Api.Models;
using EcommerceGym.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Roles.AnyAsync()) return;

        // Roles y usuarios de prueba
        var rolAdmin = new Rol { Nombre = "Administrador" };
        var rolCliente = new Rol { Nombre = "Cliente" };
        db.Roles.AddRange(rolAdmin, rolCliente);

        db.Usuarios.AddRange(
            new Usuario
            {
                Nombre = "Admin",
                Apellido = "Sistema",
                Telefono = "0000000000",
                Email = "admin@ecommercegym.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Rol = rolAdmin
            },
            new Usuario
            {
                Nombre = "Cliente",
                Apellido = "Demo",
                Telefono = "0000000000",
                Email = "cliente@ecommercegym.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente123!"),
                Rol = rolCliente
            });

        // Subcategorías
        var cintas = new Subcategoria { Nombre = "Cintas de correr" };
        var bicicletas = new Subcategoria { Nombre = "Bicicletas" };
        var maquinas = new Subcategoria { Nombre = "General" };
        var hexagonales = new Subcategoria { Nombre = "Mancuernas hexagonales" };
        var pesasRusas = new Subcategoria { Nombre = "Pesas rusas" };
        var discos = new Subcategoria { Nombre = "General" };
        var barras = new Subcategoria { Nombre = "General" };
        var racks = new Subcategoria { Nombre = "General" };
        var bancosPlanos = new Subcategoria { Nombre = "Bancos planos" };
        var bancosRegulables = new Subcategoria { Nombre = "Bancos regulables" };
        var accesorios = new Subcategoria { Nombre = "General" };
        var ropa = new Subcategoria { Nombre = "General" };

        // Categorías
        db.Categorias.AddRange(
            new Categoria { Nombre = "Cardio", Subcategorias = { cintas, bicicletas } },
            new Categoria { Nombre = "Máquinas", Subcategorias = { maquinas } },
            new Categoria { Nombre = "Mancuernas y Kettlebells", Subcategorias = { hexagonales, pesasRusas } },
            new Categoria { Nombre = "Discos Olímpicos", Subcategorias = { discos } },
            new Categoria { Nombre = "Barras", Subcategorias = { barras } },
            new Categoria { Nombre = "Racks", Subcategorias = { racks } },
            new Categoria { Nombre = "Bancos", Subcategorias = { bancosPlanos, bancosRegulables } },
            new Categoria { Nombre = "Accesorios", Subcategorias = { accesorios } },
            new Categoria { Nombre = "Ropa Deportiva", Subcategorias = { ropa } });

        // Catálogos para las relaciones N:M (se vinculan más adelante)
        db.Disciplinas.AddRange(
            new Disciplina { Nombre = "Musculación" }, new Disciplina { Nombre = "CrossFit" },
            new Disciplina { Nombre = "Powerlifting" }, new Disciplina { Nombre = "Cardio" },
            new Disciplina { Nombre = "Funcional" });
        db.GruposMusculares.AddRange(
            new GrupoMuscular { Nombre = "Pecho" }, new GrupoMuscular { Nombre = "Espalda" },
            new GrupoMuscular { Nombre = "Piernas" }, new GrupoMuscular { Nombre = "Hombros" },
            new GrupoMuscular { Nombre = "Brazos" }, new GrupoMuscular { Nombre = "Core" });

        // Productos
        Producto P(string nombre, decimal precio, int stock, Subcategoria sub,
                   UsoProducto? uso, MaterialProducto? material, decimal? oferta = null) =>
            new()
            {
                Nombre = nombre,
                Precio = precio,
                Stock = stock,
                Subcategoria = sub,
                Uso = uso,
                Material = material,
                PrecioOferta = oferta
            };

        db.Productos.AddRange(
            P("Caminadora Pro", 650000, 8, cintas, UsoProducto.Gimnasio, MaterialProducto.Acero),
            P("Caminadora Plegable", 380000, 12, cintas, UsoProducto.HomeGym, MaterialProducto.Acero, 304000),
            P("Bicicleta Spinning", 290000, 10, bicicletas, UsoProducto.Mixto, MaterialProducto.Acero),
            P("Pec Dec", 850000, 4, maquinas, UsoProducto.Gimnasio, MaterialProducto.Acero),
            P("Lat Pulldown", 780000, 5, maquinas, UsoProducto.Gimnasio, MaterialProducto.Acero, 624000),
            P("Hack Squat", 920000, 3, maquinas, UsoProducto.Gimnasio, MaterialProducto.Acero),
            P("Mancuerna Hexagonal 10 kg", 45000, 40, hexagonales, UsoProducto.Mixto, MaterialProducto.HierroFundido),
            P("Mancuerna Hexagonal 20 kg", 90000, 25, hexagonales, UsoProducto.Mixto, MaterialProducto.HierroFundido, 72000),
            P("Pesa Rusa 12 kg", 38000, 30, pesasRusas, UsoProducto.HomeGym, MaterialProducto.HierroFundido),
            P("Disco Olímpico 20 kg", 110000, 20, discos, UsoProducto.Gimnasio, MaterialProducto.HierroFundido),
            P("Barra Olímpica 20 kg", 180000, 10, barras, UsoProducto.Gimnasio, MaterialProducto.Acero),
            P("Rack de Sentadillas", 520000, 6, racks, UsoProducto.Mixto, MaterialProducto.Acero, 416000),
            P("Banco Plano", 150000, 9, bancosPlanos, UsoProducto.Mixto, MaterialProducto.Acero),
            P("Banco Regulable", 230000, 7, bancosRegulables, UsoProducto.Mixto, MaterialProducto.Acero),
            P("Agarres de Polea", 25000, 50, accesorios, UsoProducto.Gimnasio, MaterialProducto.Acero),
            P("Remera Deportiva", 22000, 60, ropa, null, MaterialProducto.Poliester));

        await db.SaveChangesAsync();
    }
}