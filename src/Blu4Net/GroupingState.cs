using System;
using System.Collections.Generic;
using Blu4Net.Channel;

namespace Blu4Net
{
    public class GroupingState
    {
        public GroupingRole Role { get; }
        public ChannelMode? ChannelMode { get; }
        public IReadOnlyList<GroupNode> GroupNodes { get; }

        public GroupingState(SyncStatusResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            ChannelMode = response.ChannelMode;

            Role = GroupingRole.None;
            if (response.Master != null)
                Role = GroupingRole.Slave;
            else if ((response.Slave != null && response.Slave.Length > 0) || response.ZoneSlave != null)
                Role = GroupingRole.Master;

            List<GroupNode> nodes = new List<GroupNode>();
            if (response.Master != null)
                nodes.Add(new GroupNode(response.Master));

            if (response.ZoneSlave != null)
                nodes.Add(new GroupNode(response.ZoneSlave));

            foreach (var slave in response.Slave ?? Array.Empty<Slave>())
                nodes.Add(new GroupNode(slave));

            GroupNodes = nodes.AsReadOnly();
        }
    }

    public class GroupNode
    {
        public GroupingRole Role { get; }
        public Uri Endpoint { get; }
        public ChannelMode? ChannelMode { get; }

        public GroupNode(Master master)
        {
            Role = GroupingRole.Master;
            Endpoint = new UriBuilder("http", master.Address, master.Port).Uri;
        }

        public GroupNode(ZoneSlave zoneSlave)
        {
            Role = GroupingRole.Slave;
            ChannelMode = zoneSlave.ChannelMode;
            Endpoint = new UriBuilder("http", zoneSlave.Address, zoneSlave.Port).Uri;
        }

        public GroupNode(Slave slave)
        {
            Role = GroupingRole.Slave;
            Endpoint = new UriBuilder("http", slave.Address, slave.Port).Uri;
        }
    }

    public enum GroupingRole
    {
        None,
        Master,
        Slave
    }
}
