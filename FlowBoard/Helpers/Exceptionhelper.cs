using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowBoard.Helpers
{
    public class Exceptionhelper
    {
        public static T CoalesceException<T>(Func<T> func, T defaultValue = default(T))
        {
            try
            {
                return func();
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}
