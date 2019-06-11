using Bogus;
using Discans.Modules;
using Discans.Resources;
using Discans.Resources.Modules;
using FakeItEasy;
using NUnit.Framework;

namespace Discans.Tests.Discans.Modules
{
    public class InfoModuleTests
    {
        protected InfoModule InfoModule;
        protected LocaledResourceManager<InfoModuleResource> LocaledResourceManager;

        [SetUp]
        public void BaseSetUp()
        {
            LocaledResourceManager = A.Fake<LocaledResourceManager<InfoModuleResource>>();
            InfoModule = A.Fake<InfoModule>(x => x.WithArgumentsForConstructor(new[] { LocaledResourceManager }));
        }

        public class InfoTests : InfoModuleTests
        {
            [SetUp]
            public void SetUp() => 
                A.CallTo(() => InfoModule.Info()).CallsBaseMethod();

            [Test]
            public void ShouldGetExpectedResource()
            {
                InfoModule.Info();
                A.CallTo(() => LocaledResourceManager.GetString(nameof(InfoModuleResource.InfoMessage)))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public void ShouldSendExpectedMessage()
            {
                const string expectedMessage = "a message {0}";
                var expectedUserName = new Faker().Person.FirstName;
                var expectedResult = $"a message {expectedUserName}";

                A.CallTo(() => LocaledResourceManager.GetString(nameof(InfoModuleResource.InfoMessage)))
                    .Returns(expectedMessage);

                A.CallTo(() => InfoModule.CurrentUserName)
                    .Returns(expectedUserName);

                InfoModule.Info();
                A.CallTo(() => InfoModule.ReplyAsync(expectedResult, false, null, null))
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
