using System;

namespace DataModel
{
    public class UserDto
    {
        public UserDto()
        {
            Licence = new LicenceDTO();
            Company = new CompanyDTO();
            Key = new CredentialKeyDTO();
        }
        public int Id { get; set; }
        public LicenceDTO Licence { get; set; } = new LicenceDTO(); 

        public CompanyDTO Company { get; set; } = new CompanyDTO();
        public CredentialKeyDTO Key { get; set; } = new CredentialKeyDTO();
        public string ProcessorId { get; set; }

        public string SymfoniaUserName { get; set; }
        public string Email { get; set; }

        public DateTime StartDat { get; set; }

        public DateTime? ExpirationDay { get; set; }

    }
}