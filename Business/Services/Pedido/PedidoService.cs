using BEL;
using DAL.Repository.Pedido;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Business.Services.Pedido
{
    public class PedidoService
    {
        private readonly PedidoRepository _repository;

        public PedidoService()
        {
            _repository = new PedidoRepository();
        }

        public BEL.Pedido CrearPedido(int idUsuario, IEnumerable<CarritoItem> items, string direccionEnvio)
        {
            if (idUsuario <= 0)
            {
                throw new InvalidOperationException("Debe iniciar sesion para finalizar la compra.");
            }

            if (string.IsNullOrWhiteSpace(direccionEnvio))
            {
                throw new ArgumentException("Debe ingresar una direccion de envio.");
            }

            if (direccionEnvio.Trim().Length > 220)
            {
                throw new ArgumentException("La direccion de envio no puede superar los 220 caracteres.");
            }

            return _repository.CrearPedido(idUsuario, items, direccionEnvio);
        }

        public void GuardarCarritoPendiente(int idUsuario, IEnumerable<CarritoItem> items)
        {
            if (idUsuario <= 0)
            {
                throw new InvalidOperationException("Debe iniciar sesion para guardar el carrito.");
            }

            _repository.GuardarCarritoPendiente(idUsuario, items);
        }

        public IList<CarritoItem> ObtenerCarritoPendiente(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                return new List<CarritoItem>();
            }

            return _repository.ObtenerCarritoPendiente(idUsuario);
        }

        public IList<BEL.Pedido> ObtenerFinalizados()
        {
            return _repository.ObtenerFinalizados();
        }

        public IList<BEL.Pedido> ObtenerFinalizadosPorUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                return new List<BEL.Pedido>();
            }

            return _repository.ObtenerFinalizadosPorUsuario(idUsuario);
        }
    }
}
