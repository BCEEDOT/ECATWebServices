using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ecat.Business.Utilities
{
    public class Retrier <TResult>
    {
        public async Task<TResult> Try(Func<Task<TResult>> task, int maxTries)
        {
            var returnResult = default(TResult);
            var tryNumber = 0;
            var delay = 5;
            var success = false;
            do
            {
                try
                {
                    returnResult = await task().ConfigureAwait(false);
                    success = true;
                }
                catch (Exception ex)
                {
                    tryNumber += 1;
                    if (tryNumber > maxTries || !IsTransient(ex)) { throw; }

                    await Task.Delay(TimeSpan.FromSeconds(delay)).ConfigureAwait(false);
                    delay += 5;
                }
            } while (tryNumber <= maxTries && !success);

            return returnResult;
        }

        private static bool IsTransient(Exception ex)
        {
            // Determine if the exception is transient.
            // In some cases this may be as simple as checking the exception type, in other 
            // cases it may be necessary to inspect other properties of the exception.
            if (ex is ArgumentException) { return true; }
            if (ex.Message.Contains("WS")) { return true; }

            var webException = ex as WebException;
            if (webException != null)
            {
                // If the web exception contains one of the following status values 
                // it may be transient.
                return new[] {WebExceptionStatus.ConnectionClosed,
                  WebExceptionStatus.Timeout,
                  WebExceptionStatus.RequestCanceled }.
                        Contains(webException.Status);
            }
            // Additional exception checking logic goes here.
            return false;
        }
    }
}
