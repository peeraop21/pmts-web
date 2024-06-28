using System;
using System.Runtime.CompilerServices;

namespace DataAccess.Utils
{
    public static class CurrentCaller
    {
        public static string GetInfo(Type type, [CallerMemberName] string caller = null) => $"{type.Name} -> {caller}";
    }
}
