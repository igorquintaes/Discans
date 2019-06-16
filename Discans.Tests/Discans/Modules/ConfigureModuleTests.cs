using Discans.Attributes;
using Discans.Modules;
using Discans.Resources;
using Discans.Resources.Modules;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discans.Tests.Extensions;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Discans.Tests.Discans.Modules
{
    public class ConfigureModuleTests : BaseTests
    {
        protected ConfigureModule Module;

        protected AppDbContext AppDbContext;
        protected ChannelService ChannelService;
        protected UserLocalizerService UserLocalizerService;
        protected ServerLocalizerService ServerLocalizerService;
        protected LocaledResourceManager<ConfigureModuleResource> LocaledResourceManager;

        [SetUp]
        public void HierarchySetUp()
        {
            AppDbContext = A.Fake<AppDbContext>();
            ChannelService = A.Fake<ChannelService>();
            UserLocalizerService = A.Fake<UserLocalizerService>();
            ServerLocalizerService = A.Fake<ServerLocalizerService>();
            LocaledResourceManager = A.Fake<LocaledResourceManager<ConfigureModuleResource>>();

            Module = A.Fake<ConfigureModule>(x => 
                x.WithArgumentsForConstructor(new object[] 
                {
                    ChannelService,
                    UserLocalizerService,
                    ServerLocalizerService,
                    AppDbContext,
                    LocaledResourceManager
                }));
        }

        public class ChannelTests : ConfigureModuleTests
        {
            private ulong guildId;
            private ulong channelId;

            [SetUp]
            public void SetUp()
            {
                guildId = Faker.Random.ULong();
                channelId = Faker.Random.ULong();

                A.CallTo(() => Module.GuildId).Returns(guildId);
                A.CallTo(() => Module.ChannelId).Returns(channelId);
            }

            [Test]
            public void ShouldBeDiscordCommand() =>
                Module.GetType().Should().BeCommand(
                    methodName: nameof(Module.SetChannel),
                    commandName: ConfigureModule.ChannelCommand);

            [Test]
            public void ShouldBeAdminRestrict() =>
                Module.GetType().Should().BeAdminRestrict(
                    methodName: nameof(Module.SetChannel));

            [Test]
            public void ShouldBeContextRestrict() =>
                Module.GetType().Should().BeContextRestrict(
                    methodName: nameof(Module.SetChannel),
                    allowedContext: ContextType.Guild);

            [Test]
            public async Task ShouldCallSaveOrUpdateOnce()
            {
                await Module.SetChannel();
                A.CallTo(() => ChannelService.SaveOrUpdate(guildId, channelId))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task ShouldCommitOnce()
            {
                await Module.SetChannel();
                A.CallTo(() => AppDbContext.SaveChangesAsync(default))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task ShouldReplyOnce()
            {
                await Module.SetChannel();
                A.CallTo(() => Module.ReplyAsync(A<string>.Ignored))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task ShouldSendExpectedResourceAsReply()
            {
                var replyMessage = Faker.Lorem.Paragraph();
                A.CallTo(() => LocaledResourceManager.GetString(nameof(ConfigureModuleResource.ChannelSuccess)))
                    .Returns(replyMessage);

                await Module.SetChannel();

                A.CallTo(() => LocaledResourceManager.GetString(nameof(ConfigureModuleResource.ChannelSuccess)))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => Module.ReplyAsync(replyMessage))
                    .MustHaveHappenedOnceExactly();
            }
        }

        public class LanguageTests : ConfigureModuleTests
        {
            private ulong guildId;
            private ulong userId;
            private string language;

            [SetUp]
            public void LanguageSetUp()
            {
                guildId = Faker.Random.ULong();
                userId = Faker.Random.ULong();
                language = LanguageService.AllowedLanguages.First();

                A.CallTo(() => Module.GuildId).Returns(guildId);
                A.CallTo(() => Module.UserId).Returns(userId);
            }

            public class ImplementationTests : LanguageTests
            {

                [Test]
                public void ShouldBeDiscordCommand() =>
                    Module.GetType().Should().BeCommand(
                        methodName: nameof(Module.Language),
                        commandName: ConfigureModule.LanguageCommand);

                [Test]
                public void ShouldBeAdminRestrict() =>
                    Module.GetType().Should().BeAdminRestrict(
                        methodName: nameof(Module.Language));

                [Test]
                public void ShouldBeContextRestrict() =>
                    Module.GetType().Should().BeContextRestrict(
                        methodName: nameof(Module.Language),
                        allowedContext: ContextType.Guild | ContextType.DM);
            }

            public class InvalidLanguageTests : LanguageTests
            {

                [Test]
                public async Task ShouldReplyUnsupported()
                {
                    var unsupportedMessage = Faker.Lorem.Paragraph();
                    A.CallTo(() => LocaledResourceManager.GetString(nameof(ConfigureModuleResource.LanguageUnsupported)))
                        .Returns(unsupportedMessage);

                    language = "InvalidLanguage";
                    await Module.Language(language);

                    A.CallTo(() => LocaledResourceManager.GetString(nameof(ConfigureModuleResource.LanguageUnsupported)))
                        .MustHaveHappenedOnceExactly();
                    A.CallTo(() => Module.ReplyAsync(A<string>.That.Contains(unsupportedMessage)))
                        .MustHaveHappenedOnceExactly();
                }

                [Test]
                public async Task ShouldNotCallAnyServiceMethods()
                {
                    language = "InvalidLanguage";
                    await Module.Language(language);

                    A.CallTo(() => UserLocalizerService.CreateOrUpdate(A<ulong>.Ignored, A<string>.Ignored))
                        .MustNotHaveHappened();
                    A.CallTo(() => ServerLocalizerService.CreateOrUpdate(A<ulong>.Ignored, A<string>.Ignored))
                        .MustNotHaveHappened();
                    A.CallTo(() => AppDbContext.SaveChangesAsync(default))
                        .MustNotHaveHappened();
                }
            }

            public class ValidLanguageTests : LanguageTests
            {
                public class InsideGuildContextTests : ValidLanguageTests
                {
                    [SetUp]
                    public void SetUp() => 
                        A.CallTo(() => Module.Channel).Returns(
                            A.Fake<ISocketMessageChannel>(builder =>
                                builder.Implements<IGuildChannel>()));

                    [Test]
                    public async Task ShouldUpdateServerLanguage()
                    {
                        await Module.Language(language);
                        A.CallTo(() => ServerLocalizerService.CreateOrUpdate(guildId, language))
                            .MustHaveHappenedOnceExactly();
                    }

                    [Test]
                    public async Task ShouldNotUpdateUserLanguage()
                    {
                        await Module.Language(language);
                        A.CallTo(() => UserLocalizerService.CreateOrUpdate(userId, language))
                            .MustNotHaveHappened();
                    }

                    [Test]
                    public async Task ShouldCommitChangesOnce()
                    {
                        await Module.Language(language);
                        A.CallTo(() => AppDbContext.SaveChangesAsync(default))
                            .MustHaveHappenedOnceExactly();
                    }

                    [Test]
                    public async Task ShouldSendExpectedResourceAsReply()
                    {
                        var resourceName = nameof(ConfigureModuleResource.LanguageUpdated);
                        var resourceCulture = CultureInfo.GetCultureInfo(language);
                        var resourceMessage = Faker.Lorem.Paragraph();
                        A.CallTo(() => LocaledResourceManager.GetString(resourceName, resourceCulture))
                            .Returns(resourceMessage);

                        await Module.Language(language);

                        A.CallTo(() => LocaledResourceManager.GetString(resourceName, resourceCulture))
                            .MustHaveHappenedOnceExactly();
                        A.CallTo(() => Module.ReplyAsync(A<string>.That.Contains(resourceMessage)))
                            .MustHaveHappenedOnceExactly();
                    }
                }

                public class InsideDmContextTests : ValidLanguageTests
                {
                    [SetUp]
                    public void SetUp() =>
                        A.CallTo(() => Module.Channel).Returns(
                            A.Fake<ISocketMessageChannel>(builder =>
                                builder.Implements<IDMChannel>()));

                    [Test]
                    public async Task ShouldUpdateUserLanguage()
                    {
                        await Module.Language(language);
                        A.CallTo(() => UserLocalizerService.CreateOrUpdate(userId, language))
                            .MustHaveHappenedOnceExactly();
                    }

                    [Test]
                    public async Task ShouldNotUpdateServerLanguage()
                    {
                        await Module.Language(language);
                        A.CallTo(() => ServerLocalizerService.CreateOrUpdate(guildId, language))
                            .MustNotHaveHappened();
                    }

                    [Test]
                    public async Task ShouldCommitChangesOnce()
                    {
                        await Module.Language(language);
                        A.CallTo(() => AppDbContext.SaveChangesAsync(default))
                            .MustHaveHappenedOnceExactly();
                    }

                    [Test]
                    public async Task ShouldSendExpectedResourceAsReply()
                    {
                        var resourceName = nameof(ConfigureModuleResource.LanguageUpdated);
                        var resourceCulture = CultureInfo.GetCultureInfo(language);
                        var resourceMessage = Faker.Lorem.Paragraph();
                        A.CallTo(() => LocaledResourceManager.GetString(resourceName, resourceCulture))
                            .Returns(resourceMessage);

                        await Module.Language(language);

                        A.CallTo(() => LocaledResourceManager.GetString(resourceName, resourceCulture))
                            .MustHaveHappenedOnceExactly();
                        A.CallTo(() => Module.ReplyAsync(A<string>.That.Contains(resourceMessage)))
                            .MustHaveHappenedOnceExactly();
                    }
                }
            }
        }
    }
}
