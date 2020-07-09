using System;
using System.Threading.Tasks;
using Polly;

namespace CliService.Infra.Common.RetryPolicy
{
    public static class RetryPolicy
    {
        public static Task ExecuteWithRetryAsync(Func<Task> func)
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryForeverAsync(IncrementRetryAttempt)
                .ExecuteAsync(func);
        }
        
        public static Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> func)
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryForeverAsync(IncrementRetryAttempt)
                .ExecuteAsync(func);
        }
        
        private static TimeSpan IncrementRetryAttempt(int attempt)
        {
            return TimeSpan.FromSeconds(Math.Pow(2, attempt));
        }
    }
}