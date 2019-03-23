using Microsoft.Extensions.Configuration;
using System;

namespace Discans.Shared
{
    public static class Helpers
    {
        public static string EnvironmentVar(IConfiguration config, string variableName)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() == "production")
                return Environment.GetEnvironmentVariable(variableName);

            return config[variableName];
        }
    }
}
