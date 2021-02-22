using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdwardHsu.Lab.RequestQueuing.ActionFilters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class RequestQueueAttribute : Attribute
    {
        public const string GLOBAL = "__GLOBAL__";
        public string Identifier { get; set; } = GLOBAL;
    }
}
