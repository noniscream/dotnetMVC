using System;
using System.Collections.Generic;

namespace Graphiczone.Models.SQLServer
{
    public partial class User
    {
        public User()
        {
            Shipping = new HashSet<Shipping>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserFirstname { get; set; }
        public string UserLastname { get; set; }
        public string UserUsername { get; set; }
        public string UserPassword { get; set; }
        public string UserAddress { get; set; }
        public string UserTel { get; set; }
        public string UserEmail { get; set; }
        public string UserPosition { get; set; }
        public double? UserSalary { get; set; }
        public int? UserStatus { get; set; }

        public virtual ICollection<Shipping> Shipping { get; set; }
    }
}
