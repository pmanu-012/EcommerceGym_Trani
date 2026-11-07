using EcommerceGym.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceGym.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Subcategoria> Subcategorias => Set<Subcategoria>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<GrupoMuscular> GruposMusculares => Set<GrupoMuscular>();
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<ProductoDisciplina> ProductoDisciplinas => Set<ProductoDisciplina>();
    public DbSet<ProductoGrupoMuscular> ProductoGruposMusculares => Set<ProductoGrupoMuscular>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();
    public DbSet<DetallePedido> DetallesPedido => Set<DetallePedido>();
    public DbSet<CarritoItem> CarritoItems => Set<CarritoItem>();
    public DbSet<ResenaProducto> ResenasProducto => Set<ResenaProducto>();
    public DbSet<ResenaPlataforma> ResenasPlataforma => Set<ResenaPlataforma>();
    public DbSet<IngresoMercaderia> IngresosMercaderia => Set<IngresoMercaderia>();
    public DbSet<DetalleIngreso> DetallesIngreso => Set<DetalleIngreso>();
    public DbSet<Auditoria> Auditorias => Set<Auditoria>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Sin borrado en cascada: las bajas son lógicas (Activo = false)
        foreach (var fk in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        // Índices únicos
        modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<Rol>().HasIndex(r => r.Nombre).IsUnique();
        modelBuilder.Entity<CarritoItem>().HasIndex(c => new { c.IdUsuario, c.IdProducto }).IsUnique();
        modelBuilder.Entity<ProductoDisciplina>().HasIndex(p => new { p.IdProducto, p.IdDisciplina }).IsUnique();
        modelBuilder.Entity<ProductoGrupoMuscular>().HasIndex(p => new { p.IdProducto, p.IdGrupoMuscular }).IsUnique();

        // Enums guardados como texto legible
        modelBuilder.Entity<Pedido>().Property(p => p.Estado).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Producto>().Property(p => p.Uso).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Producto>().Property(p => p.Material).HasConversion<string>().HasMaxLength(20);
        modelBuilder.Entity<Auditoria>().Property(a => a.Accion).HasConversion<string>().HasMaxLength(20);
    }
}