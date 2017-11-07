using DataModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DataLayer;
using System.Windows.Forms;
using Keys = DataLayer.Keys;
using MessageBox = System.Windows.MessageBox;

namespace DataAccess.DataAccess.AzureAccess
{
    public class AzureAccess
    {
        private BaseAccess baseAccess = new BaseAccess();


        public void CreateLicenceANdKey()
        {
            using (var dc = baseAccess.OpenConnection())
            {
                Licence ss = new Licence()
                {
                    AmountOfLoadedPoints = 1000,
                    TypeOfLicence = "Basic",
                    //AmountOfPointsToOptymalization = 15,
                    //AmountOfQueries = 125000,
                };
                Keys key = new Keys()
                {
                    KeyValue =
                        "AmIPymqijhRagSmhJbLYlh-u9J18JuZDnzlqgNDSROpssjc-M1wYSwEmv1ag8ilR",
                    TypeOfKey = "Basic",
                };
                dc.Keys.Add(key);
                dc.Licence.Add(ss);
                dc.SaveChanges();
            }
        }
        public CompanyDTO CreateCompany(CompanyDTO companyDto)
        {
            using (var dc = baseAccess.OpenConnection())
            {
                Companies dbItem = new Companies()
                {
                    AmountOfUsers = companyDto.AmountOfUsers,
                    NIP = companyDto.Nip,
                    SymfoniaSerial = companyDto.SymfoniaSerial,


                };
                dc.Companies.Add(dbItem);
                dc.SaveChanges();
                var company = (from item in dc.Companies
                               where item.NIP == companyDto.Nip
                               select item).FirstOrDefault();
                if (company != null)
                {
                    companyDto.Id = company.Id;

                }
                return companyDto;
            }

        }

        public UserDto CheckIfUserExists(CompanyDTO companyDTO, string cpuId, string userLogin)
        {
            using (var dc = baseAccess.OpenConnection())
            {
                UserDto userDto = new UserDto();

                var company = dc.Companies.FirstOrDefault(x =>
                    x.NIP == companyDTO.Nip && x.Id == companyDTO.Id && x.SymfoniaSerial == companyDTO.SymfoniaSerial);

                companyDTO.Id = company.Id;
                var users = dc.Users.ToList();
                var user = users.FirstOrDefault(x => x.ProcessorId == cpuId && x.SymfoniaUser == userLogin);
                if (user == null)
                {
                    CreateUser(company, cpuId, userLogin, dc);
                }
                else
                {
                    userDto = Common.ConvertDBUserToDTO(user);
                }
                return userDto;
            }


        }



        public void ChangeLicence(string nip, string userName)
        {

            using (var dc = baseAccess.OpenConnection())
            {

                var company = dc.Companies.FirstOrDefault(x => x.NIP == nip);
                var user = company.Users.FirstOrDefault(x => x.SymfoniaUser == userName);

                dc.SaveChanges();

            }

        }

        public UserDto CreateUser(Companies company, string cpuId, string userLogin, AzureConnection dc)
        {

            try
            {
                Random rnd = new Random();
                var licence = dc.Licence.FirstOrDefault(x => x.TypeOfLicence == "Basic");
                var Keys = dc.Keys.Where(x => x.TypeOfKey == "Basic").ToList();
                var key = Keys[rnd.Next(0, Keys.Count - 1)];

                Users databaseUser = new Users()
                {
                    ProcessorId = cpuId,
                    StartDay = DateTime.Now,
                    SymfoniaUser = userLogin,
                    CompanyId = company.Id,
                    KeyId = key.Id,
                    Licence = licence,
                    LicenceId = licence.Id,
                    Keys = key,
                    Companies = company,
                };
                dc.Users.Add(databaseUser);
                dc.SaveChanges();
                var dtoUser = Common.ConvertDBUserToDTO(databaseUser);
                return dtoUser;
            }
            catch (Exception s)
            {
                s.ToString();
                throw;
            }
        }

        public CompanyDTO CheckIfCompanyExists(string nip, string symfoniaSerial)
        {
            using (var dc = baseAccess.OpenConnection())
            {



                var item = dc.Companies.FirstOrDefault(x => x.NIP == nip && x.SymfoniaSerial == symfoniaSerial);
                if (item == null)
                {
                    return null;
                }
                CompanyDTO itemDTO = new CompanyDTO()
                {

                    Nip = item.NIP,
                    SymfoniaSerial = item.SymfoniaSerial,
                    AmountOfUsers = item.AmountOfUsers,
                    Id = item.Id

                };
                return itemDTO;

            }


        }


        //public void ReadData()
        //{
        //    using (var dc = baseAccess.OpenConnection())
        //    {

        //        //IQueryable<MapsTable> item = dc.MapsTable.Where(x => x.CreateDate.Day == 3);  IQueryable

        //        dc.MapsTable.Add();
        //        dc.SaveChanges();
        //    }
        //}
    }
}