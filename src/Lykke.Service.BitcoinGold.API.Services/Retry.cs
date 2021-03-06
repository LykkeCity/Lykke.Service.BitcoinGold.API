﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Common.Log;

namespace Lykke.Service.BitcoinGold.API.Services
{
    public class Retry
    {
        public static async Task<T> Try<T>(Func<Task<T>> action, Func<Exception, bool> exceptionFilter, int tryCount, ILog logger)
        {
            int @try = 0;
            while (true)
            {
                try
                {
                    return await action();
                }
                catch (Exception ex)
                {
                    @try++;
                    if (!exceptionFilter(ex) || @try >= tryCount)
                        throw;
                    await logger.WriteErrorAsync("Retry", "Try", null, ex);
                }
            }
        }

        public static async Task Try(Func<Task> action, Func<Exception, bool> exceptionFilter, int tryCount, ILog logger)
        {
            int @try = 0;
            while (true)
            {
                try
                {
                    await action();
                    return;
                }
                catch (Exception ex)
                {
                    @try++;
                    if (!exceptionFilter(ex) || @try >= tryCount)
                        throw;
                    await logger.WriteErrorAsync("Retry", "Try", null, ex);
                }
            }
        }
    }
}
