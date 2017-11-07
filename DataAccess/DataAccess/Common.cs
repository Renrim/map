using System;
using DataLayer;
using DataModel;

namespace DataAccess.DataAccess
{
    public class Common
    {
        public static UserDto ConvertDBUserToDTO(Users user)
        {
            try
            {
                UserDto userDto = new UserDto()
                {
                    Id = user.Id,
                    Email = user.eMail,
                    StartDat = user.StartDay,
                    ProcessorId = user.ProcessorId,
                    ExpirationDay = user.ExpirationDay,
                    SymfoniaUserName = user.SymfoniaUser,
                    Key = new CredentialKeyDTO()
                    {
                        Id = user.Id,
                        KeyValue = user.Keys.KeyValue,
                        TypeOfKey = user.Keys.TypeOfKey
                    },
                    Company = new CompanyDTO()
                    {
                        Id = user.Companies.Id,
                        Nip = user.Companies.NIP,
                        SymfoniaSerial = user.Companies.SymfoniaSerial,
                        AmountOfUsers = user.Companies.AmountOfUsers
                    },
                    Licence = new LicenceDTO()
                    {
                        Id = user.Licence.Id,
                        KindOfLicence = user.Licence.TypeOfLicence,
                        AmountOfLoadedPoints = user.Licence.AmountOfLoadedPoints,
                        AmountOfQueries = user.Licence.AmountOfQueries,
                        AmountOfShownPoints = user.Licence.AmountOfShownPoints,
                        OptymalizationPointsLimit = user.Licence.AmountOfPointsToOptymalization,
                        OptymalizationsLimit = user.Licence.AmountOfOptymalization
                    }
                };
                return userDto;
            }
            catch (Exception s)
            {
                s.ToString();
                return null;
            }
        }
    }
}