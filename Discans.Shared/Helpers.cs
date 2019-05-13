using Microsoft.Extensions.Configuration;
using System;

namespace Discans.Shared
{
    public static class Helpers
    {
        public static string EnvironmentVar(IConfiguration config, string variableName) =>
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() == "production"
                ? Environment.GetEnvironmentVariable(variableName)
                : config[variableName];
    }
}
