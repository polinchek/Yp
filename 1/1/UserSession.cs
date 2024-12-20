using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1
{
    internal class UserSession
    {
        public static int UserId { get; set; } // Идентификатор пользователя
        public static string Email { get; set; } // Email пользователя

        public static int RoleId { get; set; } // Роль пользователя

        // Метод для очистки сессии (например, при выходе из системы)
        public static void ClearSession()
        {
            UserId = 0;
            Email = null;
            RoleId = 0;
        }
    }
}
