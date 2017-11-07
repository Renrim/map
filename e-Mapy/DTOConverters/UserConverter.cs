using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using eMapy.Models.Licencing;

namespace eMapy.DTOConverters
{
    public class UserConverter
    {

        public static User DtoTOPoco(UserDto dto)
        {
            User user = new User()
            {
                Id = dto.Id,
                Key = new CredentialKey()
                {
                    Id = dto.Key.Id,
                    TypeOfKey = dto.Key.TypeOfKey,
                    KeyValue = dto.Key.KeyValue,
                },
                Licence = new Licence()
                {
                    Id = dto.Licence.Id,
                    KindOfLicence = dto.Licence.KindOfLicence,
                    AmountOfLoadedPoints = dto.Licence.AmountOfLoadedPoints,
                    AmountOfQueries = dto.Licence.AmountOfQueries,
                    AmountOfShownPoints = dto.Licence.AmountOfShownPoints,
                    OptymalizationsLimit = dto.Licence.OptymalizationsLimit,
                    OptymalizationPointsLimit = dto.Licence.OptymalizationPointsLimit,

                },
                Company = new Company()
                {
                    Id = dto.Company.Id,
                    Nip = dto.Company.Nip,
                    SymfoniaSerial = dto.Company.SymfoniaSerial,
                    AmountOfUsers = dto.Company.AmountOfUsers
                },
                Email = dto.Email,
                ProcessorId = dto.ProcessorId,
                SymfoniaUserName = dto.SymfoniaUserName,
                ExpirationDay = dto.ExpirationDay,
                StartDat = dto.StartDat
            };
            return user;
        }




    }
}
