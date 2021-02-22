using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace EdwardHsu.Lab.RequestQueuing.ActionFilters
{
    public class RequestQueueActionFilter : IAsyncActionFilter
    {
        private static ConcurrentDictionary<string, ActionBlock<(ActionExecutionDelegate, TaskCompletionSource)>> _requestQueue
            = new ConcurrentDictionary<string, ActionBlock<(ActionExecutionDelegate, TaskCompletionSource)>>();
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor cad)
            {
                var queueAttr = cad.MethodInfo.GetCustomAttribute<RequestQueueAttribute>();

                var tcs = new TaskCompletionSource();

                var queue = _requestQueue.GetOrAdd(queueAttr.Identifier, new ActionBlock<(ActionExecutionDelegate, TaskCompletionSource)>(async ((ActionExecutionDelegate _next, TaskCompletionSource _tcs) input) =>
                {
                    try
                    {
                        await input._next();
                        input._tcs.SetResult();
                    }
                    catch (Exception e)
                    {
                        input._tcs.SetException(e);
                    }
                }));

                queue.Post((next, tcs));

                await tcs.Task;
            }
            else
            {
                await next();
            }
        }
    }
}
