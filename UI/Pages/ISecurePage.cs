using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Pages
{
    internal interface ISecurePage
    {
        bool ValidarAcceso(string rol);
    }
}
