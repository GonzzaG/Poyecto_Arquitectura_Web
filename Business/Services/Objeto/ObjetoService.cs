using DAL.Repository.Objeto;
using System.Collections.Generic;

namespace Business.Services.Objeto
{
    public class ObjetoService
    {
        private readonly ObjetoRepository _repository;

        public ObjetoService()
        {
            _repository = new ObjetoRepository();
        }

        public List<BEL.Objeto> ObtenerProductos()
        {
            return _repository.ObtenerProductos();
        }

        public BEL.Objeto ObtenerPorId(int id)
        {
            return _repository.ObtenerPorId(id);
        }
    }
}
