using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBedsTask.Utils
{
    public static class ExceptionHandler
    {
        public static ActionResult HandleException(Exception srcEx)
        {
            LogException(srcEx);
            StatusCodeResult statusCodeResult;

            if (srcEx is UnauthorizedAccessException)
                statusCodeResult = new StatusCodeResult(401);
            else
                statusCodeResult = new StatusCodeResult(500);

            return statusCodeResult;
        }

        private static void LogException(Exception srcEx)
        {
            //TODO: Add the logic for logging the exception.
        }
    }
}
