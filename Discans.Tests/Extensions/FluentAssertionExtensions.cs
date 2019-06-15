using Discans.Attributes;
using Discord.Commands;
using FluentAssertions;
using FluentAssertions.Types;
using System.Linq;

namespace Discans.Tests.Extensions
{
    public static class FluentAssertionExtensions
    {
        public static void BeAdminRestrict(this TypeAssertions assertion, string methodName) => 
            assertion.Match(x =>
                x.GetMethod(methodName)
                 .Should()
                 .BeDecoratedWith<AdminAttribute>(default) != null);

        public static void NotBeAdminRestrict(this TypeAssertions assertion, string methodName) =>
            assertion.Match(x =>
                x.GetMethod(methodName)
                 .Should()
                 .NotBeDecoratedWith<AdminAttribute>(default) != null);

        public static void BeContextRestrict(this TypeAssertions assertion, string methodName, ContextType allowedContext) =>
            assertion.Match(x =>
                x.GetMethod(methodName)
                 .Should()
                 .BeDecoratedWith<LocaledRequireContextAttribute>(default) != null &&
                x.GetMethod(methodName).GetCustomAttributes(typeof(LocaledRequireContextAttribute), false)
                 .Single()
                 .GetType()
                 .GetProperty("Contexts")
                 .GetValue(x.GetMethod(methodName)
                            .GetCustomAttributes(typeof(LocaledRequireContextAttribute), false)
                            .Single(), null)
                 .As<ContextType>() == allowedContext);

        public static void NotBeContextRestrict(this TypeAssertions assertion, string methodName) =>
            assertion.Match(x =>
                x.GetMethod(methodName)
                 .Should()
                 .NotBeDecoratedWith<LocaledRequireContextAttribute>(default) != null);

        public static void BeCommand(this TypeAssertions assertion, string methodName, string commandName) =>
            assertion.Match(x =>
                x.GetMethod(methodName)
                 .Should()
                 .BeDecoratedWith<CommandAttribute>(default) != null &&
                x.GetMethod(methodName).GetCustomAttributes(typeof(CommandAttribute), false)
                 .Single()
                 .GetType()
                 .GetProperty("Text")
                 .GetValue(x.GetMethod(methodName)
                            .GetCustomAttributes(typeof(CommandAttribute), false)
                            .Single(), null)
                 .ToString() == commandName);

    }
}
