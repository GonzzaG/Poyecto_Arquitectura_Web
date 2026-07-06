using BEL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace DAL.Repository.Pedido
{
    public class PedidoRepository
    {
        public BEL.Pedido CrearPedido(int idUsuario, IEnumerable<CarritoItem> items, string direccionEnvio)
        {
            if (items == null)
            {
                throw new ArgumentException("El carrito esta vacio.");
            }

            var cantidades = items
                .Where(x => x != null && x.IdObjeto > 0 && x.Cantidad > 0)
                .GroupBy(x => x.IdObjeto)
                .ToDictionary(x => x.Key, x => x.Sum(i => i.Cantidad));

            if (!cantidades.Any())
            {
                throw new ArgumentException("El carrito esta vacio.");
            }

            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                var ids = cantidades.Keys.ToList();
                var productos = context.Objetos
                    .Where(x => ids.Contains(x.IdObjeto) && x.EsProducto)
                    .ToList();

                if (productos.Count != ids.Count)
                {
                    throw new InvalidOperationException("El carrito contiene productos no disponibles.");
                }

                foreach (var producto in productos)
                {
                    var cantidad = cantidades[producto.IdObjeto];
                    if (producto.Stock < cantidad)
                    {
                        throw new InvalidOperationException($"No hay stock suficiente para {producto.Nombre}.");
                    }
                }

                var pedido = context.Pedidos
                    .Include(x => x.Detalles)
                    .FirstOrDefault(x => x.IdUsuario == idUsuario && !x.Finalizado);

                if (pedido == null)
                {
                    pedido = new BEL.Pedido
                    {
                        IdUsuario = idUsuario,
                        FechaHora = DateTime.Now
                    };
                    context.Pedidos.Add(pedido);
                }
                else
                {
                    foreach (var detalle in pedido.Detalles.ToList())
                    {
                        context.DetallesPedido.Remove(detalle);
                    }

                    pedido.Detalles.Clear();
                }

                pedido.DireccionEnvio = direccionEnvio.Trim();
                pedido.Finalizado = true;
                pedido.Total = productos.Sum(x => x.Precio * cantidades[x.IdObjeto]);

                foreach (var producto in productos)
                {
                    var cantidad = cantidades[producto.IdObjeto];
                    producto.Stock -= cantidad;

                    pedido.Detalles.Add(new DetallePedido
                    {
                        IdObjeto = producto.IdObjeto,
                        Cantidad = cantidad,
                        Finalizado = true
                    });

                    context.Entry(producto).State = EntityState.Modified;
                }

                context.Pedidos.Add(pedido);
                context.SaveChanges();
                transaction.Commit();

                return pedido;
            }
        }

        public void GuardarCarritoPendiente(int idUsuario, IEnumerable<CarritoItem> items)
        {
            var cantidades = (items ?? Enumerable.Empty<CarritoItem>())
                .Where(x => x != null && x.IdObjeto > 0 && x.Cantidad > 0)
                .GroupBy(x => x.IdObjeto)
                .ToDictionary(x => x.Key, x => x.Sum(i => i.Cantidad));

            using (var context = new AppDbContext())
            using (var transaction = context.Database.BeginTransaction())
            {
                var pedido = context.Pedidos
                    .Include(x => x.Detalles)
                    .FirstOrDefault(x => x.IdUsuario == idUsuario && !x.Finalizado);

                if (!cantidades.Any())
                {
                    if (pedido != null)
                    {
                        context.Pedidos.Remove(pedido);
                        context.SaveChanges();
                    }

                    transaction.Commit();
                    return;
                }

                var ids = cantidades.Keys.ToList();
                var productos = context.Objetos
                    .Where(x => ids.Contains(x.IdObjeto) && x.EsProducto)
                    .ToList();

                if (productos.Count != ids.Count)
                {
                    throw new InvalidOperationException("El carrito contiene productos no disponibles.");
                }

                if (pedido == null)
                {
                    pedido = new BEL.Pedido
                    {
                        IdUsuario = idUsuario,
                        FechaHora = DateTime.Now,
                        DireccionEnvio = "Pendiente de confirmacion",
                        Finalizado = false
                    };
                    context.Pedidos.Add(pedido);
                }
                else
                {
                    foreach (var detalle in pedido.Detalles.ToList())
                    {
                        context.DetallesPedido.Remove(detalle);
                    }

                    pedido.Detalles.Clear();
                }

                pedido.Total = productos.Sum(x => x.Precio * cantidades[x.IdObjeto]);
                pedido.FechaHora = DateTime.Now;

                foreach (var producto in productos)
                {
                    pedido.Detalles.Add(new DetallePedido
                    {
                        IdObjeto = producto.IdObjeto,
                        Cantidad = cantidades[producto.IdObjeto],
                        Finalizado = false
                    });
                }

                context.SaveChanges();
                transaction.Commit();
            }
        }

        public List<CarritoItem> ObtenerCarritoPendiente(int idUsuario)
        {
            using (var context = new AppDbContext())
            {
                var pedido = context.Pedidos
                    .Include(x => x.Detalles.Select(d => d.Objeto))
                    .Where(x => x.IdUsuario == idUsuario && !x.Finalizado)
                    .OrderByDescending(x => x.FechaHora)
                    .FirstOrDefault();

                if (pedido == null)
                {
                    return new List<CarritoItem>();
                }

                return pedido.Detalles
                    .OrderBy(x => x.Objeto.Nombre)
                    .Select(x => new CarritoItem
                    {
                        IdObjeto = x.IdObjeto,
                        Nombre = x.Objeto.Nombre,
                        Precio = x.Objeto.Precio,
                        ImagenUrl = x.Objeto.ImagenUrl,
                        Cantidad = x.Cantidad
                    })
                    .ToList();
            }
        }

        public List<BEL.Pedido> ObtenerFinalizados()
        {
            using (var context = new AppDbContext())
            {
                return context.Pedidos
                    .Include(x => x.Usuario)
                    .Include(x => x.Detalles.Select(d => d.Objeto))
                    .Where(x => x.Finalizado)
                    .OrderByDescending(x => x.FechaHora)
                    .ToList();
            }
        }

        public List<BEL.Pedido> ObtenerFinalizadosPorUsuario(int idUsuario)
        {
            using (var context = new AppDbContext())
            {
                return context.Pedidos
                    .Include(x => x.Usuario)
                    .Include(x => x.Detalles.Select(d => d.Objeto))
                    .Where(x => x.Finalizado && x.IdUsuario == idUsuario)
                    .OrderByDescending(x => x.FechaHora)
                    .ToList();
            }
        }
    }
}
