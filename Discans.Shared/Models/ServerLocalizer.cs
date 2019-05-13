using System;

namespace Discans.Shared.Models
{
    public class ServerLocalizer : Localizer
    {
        public ServerLocalizer(ulong serverId, string language)  
            : base(language) => 
                ServerId = serverId;

        public ulong ServerId { get; protected set; }

    }
}
