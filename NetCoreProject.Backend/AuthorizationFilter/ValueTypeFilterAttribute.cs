using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreProject.Backend.AuthorizationFilter
{
    public class ValueTypeFilterAttribute : TypeFilterAttribute
    {
        public ValueTypeFilterAttribute(params string[] values)
            : base(typeof(ValueAuthorizationFilter))
        {
            Arguments = new object[] { values };
        }
    }
}
