﻿namespace FlashTrashMob.Web.Common
{
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Newtonsoft.Json.Linq;

    public class ValidateModelFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var errors = new JObject();
                foreach (var key in context.ModelState.Keys)
                {
                    var state = context.ModelState[key];
                    if (state.Errors.Count > 0)
                    {
                        errors[key] = state.Errors[0].ErrorMessage;
                    }
                }

                context.Result = new ObjectResult(errors);
            }
            else
            { 
                await next();
            }
        }
    }
}
