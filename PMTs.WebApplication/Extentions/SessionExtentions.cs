using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PMTs.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Extentions
{
    public static class SessionExtentions
    {
        public static void SetSession(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }

        public static T GetSession<T>(this ISession session, string key)
        {
            //   return JsonConvert.DeserializeObject<T>(session.GetString(key));
            var value = session.GetString(key);
            return value == null ? default(T) :
                                  JsonConvert.DeserializeObject<T>(value);
        }

    }
}
