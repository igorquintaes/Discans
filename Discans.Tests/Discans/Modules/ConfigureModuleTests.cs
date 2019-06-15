using Discans.Attributes;
using Discans.Modules;
using Discans.Resources;
using Discans.Resources.Modules;
using Discans.Shared.Database;
using Discans.Shared.Services;
using Discans.Tests.Extensions;
using Discord.Commands;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
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
                    methodName: nameof(Module.Channel),
                    commandName: ConfigureModule.ChannelCommand);

            [Test]
            public void ShouldBeAdminRestrict() =>
                Module.GetType().Should().BeAdminRestrict(
                    methodName: nameof(Module.Channel));

            [Test]
            public void ShouldBeContextRestrict() =>
                Module.GetType().Should().BeContextRestrict(
                    methodName: nameof(Module.Channel),
                    allowedContext: ContextType.Guild);

            [Test]
            public async Task ShouldCallSaveOrUpdateOnce()
            {
                await Module.Channel();
                A.CallTo(() => ChannelService.SaveOrUpdate(guildId, channelId))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task ShouldCommitOnce()
            {
                await Module.Channel();
                A.CallTo(() => AppDbContext.SaveChangesAsync(default))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task ShouldReplyOnce()
            {
                await Module.Channel();
                A.CallTo(() => Module.ReplyAsync(A<string>.Ignored, default, default, default))
                    .MustHaveHappenedOnceExactly();
            }

            [Test]
            public async Task ShouldSendExpectedResourceAsReply()
            {
                var replyMessage = Faker.Lorem.Paragraph();
                A.CallTo(() => LocaledResourceManager.GetString(nameof(ConfigureModuleResource.ChannelSuccess)))
                    .Returns(replyMessage);

                await Module.Channel();

                A.CallTo(() => LocaledResourceManager.GetString(nameof(ConfigureModuleResource.ChannelSuccess)))
                    .MustHaveHappenedOnceExactly();
                A.CallTo(() => Module.ReplyAsync(replyMessage, default, default, default))
                    .MustHaveHappenedOnceExactly();
            }
        }
    }
}
