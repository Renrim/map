using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMapy.Models.Licencing
{
   public class User
    {
        public User()
        {
            Licence = new Licence();
            Key = new CredentialKey();
            Company = new Company();
        }
        public int Id { get; set; }
        public Licence Licence { get; set; }

        public Company Company { get; set; }
        public CredentialKey Key { get; set; }
        public string ProcessorId { get; set; }

        public string SymfoniaUserName { get; set; }
        public string Email { get; set; }

        public DateTime StartDat { get; set; }

        public DateTime? ExpirationDay { get; set; }

    }
}
