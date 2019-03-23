namespace Discans.Shared.Models
{
    public class ServerChannel
    {
        protected ServerChannel()
        { }

        public ServerChannel(ulong serverId, ulong channelId)
        {
            Id = serverId;
            ChannelId = channelId;
        }

        public ulong Id { get; protected set; }
        public ulong ChannelId { get; protected set; }

        public void UpdateChannel(ulong channelId) => 
            ChannelId = channelId;
    }
}
