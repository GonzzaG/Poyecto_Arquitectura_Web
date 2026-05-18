using BEL.Constantes;
using System.Collections.Generic;

namespace UI.Pages
{
    internal interface ISecurePage
    {
        bool ValidarAcceso(List<RolesEnum> roles);
    }
}
