using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abyss_Bot
{
    public class UserData
    {
        public ulong guildId { get; set; }
        public int exp { get; set; }
        public int lvl{ get; set; }

        public UserData(ulong guildId, int exp, int lvl)
        {
            this.guildId = guildId;
            this.exp = exp;
            this.lvl = lvl;
        }
    }
}
