using System.Collections.Generic;
using System.Linq;

namespace DAL.Repository.Objeto
{
    public class ObjetoRepository
    {
        public List<BEL.Objeto> ObtenerProductos()
        {
            using (var context = new AppDbContext())
            {
                return context.Objetos
                    .Where(o => o.EsProducto)
                    .OrderBy(o => o.Nombre)
                    .ToList();
            }
        }

        public BEL.Objeto ObtenerPorId(int id)
        {
            using (var context = new AppDbContext())
            {
                return context.Objetos.FirstOrDefault(o => o.IdObjeto == id);
            }
        }
    }
}
