using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlgoritmDM.Lib
{
    public class User : UserBase
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="Logon">Имя пользователя</param>
        /// <param name="Password">Пароль пользователя</param>
        /// <param name="Description">Описание пользователя</param>
        /// <param name="Role">Роль пользователя</param>
        public User(string Logon, string Password, string Description, RoleEn Role)
        {
            base.Password = Password;
            base.Description = Description;
            base.Role = Role;

            base.InitialUser(Logon);
        }


    }
}
