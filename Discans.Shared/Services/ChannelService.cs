using Discans.Shared.Database;
using Discans.Shared.Models;
using System.Threading.Tasks;

namespace Discans.Shared.Services
{
    public class ChannelService
    {
        private readonly AppDbContext context;

        public ChannelService(AppDbContext context) => 
            this.context = context;

        public virtual async Task<ServerChannel> GetByServerId(ulong serverId) =>
            await context.ServerChannels.FindAsync(serverId);

        public virtual async Task SaveOrUpdate(ulong serverId, ulong channelId)
        {
            var serverChannel = await GetByServerId(serverId);
            if (serverChannel == null)
                context.ServerChannels.Add(new ServerChannel(serverId, channelId));
            else
            {
                serverChannel.UpdateChannel(channelId);
                context.ServerChannels.Update(serverChannel);
            }
        }
    }
}
