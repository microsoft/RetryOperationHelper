using System;
using System.Threading.Tasks;

namespace Microsoft.Samples
{
    /// <summary>
    /// Provides functionality to automatically try the given piece of logic some number of times before re-throwing the exception. 
    /// This is useful for any piece of code which may experience transient failures. Be cautios of passing code with two distinct 
    /// actions given that if the second or subsequent piece of logic fails, the first will also be retried upon each retry. 
    /// </summary>
    public class RetryOperationHelper
    {
        /// <summary>Gets or sets the retry interval.</summary>
        public TimeSpan? RetryInterval { get; set; }

        /// <summary>Executes asynchronous function with retry logic.</summary>
        /// <param name="func">The asynchronous function to be executed.</param>
        /// <param name="maxAttempts">The maximum number of attempts.</param>
        /// <param name="retryInterval">Timespan to wait between attempts of the operation</param>
        /// <param name="onAttemptFailed">The callback executed when an attempt is failed.</param>
        /// <typeparam name="T">The result type.</typeparam>
        /// <returns>The <see cref="Task"/> producing the result.</returns>
        public async Task<T> ExecuteWithRetry<T>(Func<Task<T>> func, int maxAttempts, TimeSpan? retryInterval = null, Action<int, Exception> onAttemptFailed = null)
        {
            if (maxAttempts < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, "The maximum number of attempts must not be less than 1.");
            }

            var attempt = 0;

            while (true)
            {
                if (attempt > 0 && this.RetryInterval != null)
                {
                    await Task.Delay(this.RetryInterval.Value);
                }

                try
                {
                    //Call the function passed in by the caller. 
                    return await func().ConfigureAwait(false);
                }
                catch (Exception exception)
                {
                    attempt++;

                    if (onAttemptFailed != null)
                    {
                        onAttemptFailed(attempt, exception);
                    }

                    if (attempt >= maxAttempts)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>Executes asynchronous function with retry logic.</summary>
        /// <param name="func">The asynchronous function to be executed.</param>
        /// <param name="maxAttempts">The maximum number of attempts.</param>
        /// <param name="retryInterval">Timespan to wait between attempts of the operation</param>
        /// <param name="onAttemptFailed">The retry handler.</param>
        /// <returns>The <see cref="Task"/> producing the result.</returns>
        public async Task ExecuteWithRetry(Func<Task> func, int maxAttempts, TimeSpan? retryInterval = null, Action<int, Exception> onAttemptFailed = null)
        {
            Func<Task<bool>> wrapper = async () =>
            {
                await func().ConfigureAwait(false);
                return true;
            };

            await this.ExecuteWithRetry(wrapper, maxAttempts, retryInterval, onAttemptFailed);
        }
    }
}