using System;
using System.Collections.Generic;
using System.Text;
using BEL.DTOs.Bitacora;
using DAL.Repository.Bitacora;

namespace Business.Services.Bitacora
{
    public sealed class BitacoraService
    {
        private readonly BitacoraRepository _repository = new BitacoraRepository();

        public void RegistrarAccion(BitacoraRegistroDto registro)
        {
            if (registro == null)
            {
                return;
            }

            registro.FechaUtc = registro.FechaUtc == default(DateTime) ? DateTime.UtcNow : registro.FechaUtc;
            registro.TipoEvento = "ACCION";
            registro.Resultado = string.IsNullOrWhiteSpace(registro.Resultado) ? "OK" : registro.Resultado;

            _repository.Insertar(registro);
        }

        public void RegistrarError(BitacoraRegistroDto registroError)
        {
            if (registroError == null || registroError.Exception == null)
            {
                return;
            }

            var registro = new BitacoraRegistroDto
            {
                FechaUtc = DateTime.UtcNow,
                TipoEvento = "ERROR",
                Modulo = registroError.Modulo,
                Accion = registroError.Exception.GetType().Name,
                Resultado = "ERROR",
                Mensaje = ArmarMensajeError(registroError.Exception.Message, registroError.CodigoCorrelacion),
                Detalle = ConstruirDetalleError(registroError.Exception, registroError.CodigoCorrelacion),
                IdUsuario = registroError.IdUsuario,
                UsuarioEmail = registroError.UsuarioEmail,
                Url = registroError.Url,
                Ip = registroError.Ip,
                CodigoCorrelacion = registroError.CodigoCorrelacion
            };

            _repository.Insertar(registro);
        }

        public IList<BitacoraRegistroDto> ObtenerRecientes(int cantidadMaxima)
        {
            return _repository.ObtenerRecientes(cantidadMaxima);
        }

        private static string ConstruirDetalleError(Exception exception, string codigoCorrelacion)
        {
            var builder = new StringBuilder();
            var actual = exception;
            var nivel = 0;

            if (!string.IsNullOrWhiteSpace(codigoCorrelacion))
            {
                builder.AppendLine("CodigoReferencia: " + codigoCorrelacion);
                builder.AppendLine();
            }

            while (actual != null && nivel < 5)
            {
                if (nivel > 0)
                {
                    builder.AppendLine();
                }

                builder.AppendLine("Nivel " + nivel);
                builder.AppendLine("Tipo: " + actual.GetType().FullName);
                builder.AppendLine("Mensaje: " + actual.Message);
                builder.AppendLine("StackTrace:");
                builder.AppendLine(actual.StackTrace);

                actual = actual.InnerException;
                nivel++;
            }

            return builder.ToString();
        }

        private static string ArmarMensajeError(string mensajeBase, string codigoCorrelacion)
        {
            var mensaje = string.IsNullOrWhiteSpace(mensajeBase) ? "Error no controlado." : mensajeBase.Trim();
            if (string.IsNullOrWhiteSpace(codigoCorrelacion))
            {
                return mensaje;
            }

            return mensaje + " [REF:" + codigoCorrelacion + "]";
        }
    }
}
