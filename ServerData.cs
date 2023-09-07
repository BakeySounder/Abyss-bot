namespace Abyss_Bot
{
    public class ServerData
    {
        public ulong guildId { get; set; }
        public DateTime InitDate { get; set; }
        public string guildType { get; set; }

        public ServerData(ulong guildId, DateTime initDate, string guildType)
        {
            this.guildId = guildId;
            InitDate = initDate;
            this.guildType = guildType;
        }
    }
}
