using System;
using DataAccess.DataAccess.AzureAccess;
using DataModel;
using eMapy.Models.Licencing;
using eMapy.Utils;
using eMapy.ViewModels;
using WymianaFTP;

namespace eMapy.Managers
{
    public class LicenceManager : ManagerBase<MainWindowViewModel>
    {
        public static string Nip { get; set; }
        public static string SymfoniaSerial { get; set; }
        public static string UserLogin { get; set; }
        public AzureAccess DbAccess { get; set; } = new AzureAccess();
        public LicenceManager(MainWindowViewModel param) : base(param)
        {
        }


        public User CheckLicence()
        {

       

            var cpuId = WMI.GetCPUId();
            var companyDTO = CheckIfCompanyExists(Nip, SymfoniaSerial);
            var userDTO = DbAccess.CheckIfUserExists(companyDTO, cpuId, UserLogin);
            var user = DTOConverters.UserConverter.DtoTOPoco(userDTO);


            return user;


        }


        public CompanyDTO CheckIfCompanyExists(string nip, string symfoniaSerial)
        {
            var companyDto = DbAccess.CheckIfCompanyExists(nip, symfoniaSerial);
            if (companyDto == null)
            {
                CompanyDTO newCompanyDTO = new CompanyDTO()
                {

                    Nip = nip,
                    SymfoniaSerial = symfoniaSerial,
                    //AmountOfUsers = null,

                };
                newCompanyDTO = DbAccess.CreateCompany(newCompanyDTO);
                //var newCompany = CompanyConverter.DtoToPoco(companyDTO);

                return newCompanyDTO;

            }
            return companyDto;
        }

    }
}