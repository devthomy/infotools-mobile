using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adatea.classe
{
    public static class SessionManager
    {
        public static int CurrentUserId { get; set; } = -1; // Initialisé à -1 ou toute autre valeur indiquant qu'aucun utilisateur n'est connecté
    }


}
