using DataLayer;

namespace DataAccess.DataAccess.AzureAccess
{
    public class BaseAccess
    {
        public AzureConnection OpenConnection()
        {
            return new AzureConnection();
        }
    }
}