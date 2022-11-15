using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Common.Extentions
{
    public static class ControllerExt
    {
        public static string? ControllerAction<T>(this IUrlHelper urlHelper, string name, object? arg) where T : ControllerBase
        {
            var type = typeof(T);
            var method = type.GetMethod(name);
            if (method == null)
            {
                return null;
            }
            else
            {
                var controller = typeof(T).Name.Replace("Controller", string.Empty);
                var action = urlHelper.Action(name, controller, arg);
                return action;
            }
        }
    }
}
