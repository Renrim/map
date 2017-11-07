using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using eMapy.Models.Licencing;

namespace eMapy.DTOConverters
{
    public class CompanyConverter
    {
        public static CompanyDTO PocoToDto(Company company)
        {
            CompanyDTO itemDto = new CompanyDTO()
            {
                Nip = company.Nip,
                SymfoniaSerial = company.SymfoniaSerial,
                AmountOfUsers = company.AmountOfUsers,
                Id = company.Id

            };
            return itemDto;

        }

        public static Company DtoToPoco(CompanyDTO companyDto)
        {
            Company item = new Company()
            {
                Nip = companyDto.Nip,
                SymfoniaSerial = companyDto.SymfoniaSerial,
                AmountOfUsers = companyDto.AmountOfUsers,
                Id = companyDto.Id

            };
            return item;

        }

    }
}
