using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fanex.Logging;
using Fanex.OddsFeed.Common.Configurations;
using Fanex.OddsFeed.Common.Helpers;

namespace Fanex.OddsFeed.WebService.App_Start
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            var is404Error = filterContext.Exception is HttpException
                && ((HttpException)filterContext.Exception).ErrorCode == 404;

            if (!is404Error)
            {
                LogHelper.Log(filterContext.Exception);
            }

            base.OnException(filterContext);
        }
    }
}