using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eMapy.Utils
{
    public abstract class ManagerBase<T>
    {
        public T ViewModel { get; set; }

        public ManagerBase(T param)
        {
            ViewModel = param;
            
        }
    }
}