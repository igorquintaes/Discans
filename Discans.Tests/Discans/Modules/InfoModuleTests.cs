using Bogus;
using Discans.Modules;
using Discans.Resources;
using Discans.Resources.Modules;
using Discans.Tests.Extensions;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Discans.Tests.Discans.Modules
{
    public class InfoModuleTests : BaseTests
    {
        protected InfoModule Module;
        protected LocaledResourceManager<InfoModuleResource> LocaledResourceManager;

        [SetUp]
        public void HierarchySetUp()
        {
            LocaledResourceManager = A.Fake<LocaledResourceManager<InfoModuleResource>>();
            Module = A.Fake<InfoModule>(x => x.WithArgumentsForConstructor(new[] { LocaledResourceManager }));
        }

        public class InfoTests : InfoModuleTests
        {
            [Test]
            public void ShouldBeDiscordCommand() =>
                Module.GetType().Should().BeCommand(
                    methodName: nameof(Module.Info),
                    commandName: InfoModule.InfoCommand);

            [Test]
            public void ShouldNotBeAdminRestrict() =>
                Module.GetType().Should().NotBeAdminRestrict(
                    methodName: nameof(Module.Info));

            [Test]
            public async Task ShouldGetExpectedResource()
            {
                await Module.Info();
                A.CallTo(() => LocaledResourceManager.GetString(nameof(InfoModuleResource.InfoMessage)))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task ShouldSendExpectedResourceAsReply()
            {
                var resourceMessage = Faker.Lorem.Paragraph();
                A.CallTo(() => LocaledResourceManager.GetString(nameof(InfoModuleResource.InfoMessage)))
                    .Returns(resourceMessage);

                await Module.Info();

                A.CallTo(() => LocaledResourceManager.GetString(nameof(InfoModuleResource.InfoMessage)))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => Module.ReplyAsync(A<string>.That.Contains(resourceMessage)))
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
