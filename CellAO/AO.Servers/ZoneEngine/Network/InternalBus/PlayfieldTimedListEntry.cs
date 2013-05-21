using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZoneEngine.Network.InternalBus
{
    public class PlayfieldTimedListEntry : IComparable<PlayfieldTimedListEntry>
    {
        public DateTime Trigger;

        public object obj;

        public PlayfieldTimedListEntry(DateTime trigger, object obj)
        {
            this.obj = obj;
            this.Trigger = trigger;
        }

        public int CompareTo(PlayfieldTimedListEntry other)
        {
            return Trigger.CompareTo(other.Trigger);
        }
    }
}
