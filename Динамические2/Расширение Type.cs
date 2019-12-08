using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Динамические;

namespace РасширениеType
{
    public static class РасширениеType2
    {
        public static object НовыйЭкземпляр(this Type тип)
        {
            return  тип.GetConstructor(Type.EmptyTypes).Invoke(null);
        }

        public static T Экземпляр<T>()
        {
            return (T) typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null);
        }
    }
}
