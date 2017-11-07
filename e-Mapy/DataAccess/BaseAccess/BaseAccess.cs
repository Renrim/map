using DataLayer;

namespace eMapy.DataAccess.BaseAccess
{
    public class BaseAccess
    {
        public static eMapyEntities2 OpenConnection()
        {
            return new eMapyEntities2();
        }
    }
}