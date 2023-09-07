using System.Data;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Data.SqlClient;

namespace Abyss_Bot
{
    public class Database
    {
        static string connectionString = "Server=(localdb)\\mssqllocaldb;Database=AbyssDB;Trusted_Connection=True;";
        private SqlConnection connection;

        async public Task CreateTablesIfNotExists()
        {
            if(connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;

            command.CommandText = await File.ReadAllTextAsync(@"sql\InitTables.sql");
            command.CommandText.Replace("\r",null);
            command.CommandText.Replace("\n", null);
            command.ExecuteNonQuery();
            await connection.CloseAsync();
            Console.WriteLine("Таблицы созданы");
        }

        async public Task<bool> InitGuild(DiscordChannel channel)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {channel.GuildId};";
            var reader = await command.ExecuteReaderAsync();
            if (reader.HasRows)
            {
                await connection.CloseAsync();
                return false;
            }
            await reader.CloseAsync();

            var role = await channel.Guild.CreateRoleAsync("Abyss", Permissions.Administrator);
            var everyone = channel.Guild.Roles.Values.First(x=> x.Name == "@everyone");
             var channels = await channel.Guild.CreateChannelCategoryAsync("Abyss bot");
            await channels.AddOverwriteAsync(everyone, deny:Permissions.All);
            await channels.AddOverwriteAsync(role, Permissions.Administrator);
            var text_channel = await channel.Guild.CreateTextChannelAsync("Log",parent:channels);

            command.CommandText = $"INSERT INTO dbo.Guilds Values({channel.GuildId},'{DateTime.Now}','{"default"}', {channels.Id}, {text_channel.Id});";

            await command.ExecuteNonQueryAsync();
            await connection.CloseAsync();
            Console.WriteLine($"Сервер '{channel.Guild.Name}' Занесен в БД");
            return true;
            
        }

        async public Task Connect()
        {
            connection = new SqlConnection(connectionString);
            await connection.OpenAsync();
        }

        public async Task<ServerData> GetServerData(ulong? guildId)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {guildId};";

            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(dataTable);


            if (dataTable.Rows.Count ==0)
            {
                await connection.CloseAsync();
                return null;
            }

            var guildIdd = ulong.Parse(dataTable.Rows[0][0].ToString());
            var date = DateTime.Parse(dataTable.Rows[0][1].ToString());
            var type = dataTable.Rows[0][2].ToString();


            ServerData data = new ServerData(guildIdd, date, type);
            await connection.CloseAsync();
            return data;
        }

        public async Task<UserData> GetUserData(ulong? guildId, ulong userid)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {guildId};";

            var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return null;
            }
            await reader.CloseAsync();

            command.CommandText = $"SELECT * FROM dbo.Users where guild_id = {guildId} AND user_id = {userid};";

            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(dataTable);

            var useriD = ulong.Parse(dataTable.Rows[0][1].ToString());
            var exp = int.Parse(dataTable.Rows[0][2].ToString());
            var lvl = int.Parse(dataTable.Rows[0][3].ToString());



            var data = new UserData(useriD, lvl, exp);
            await connection.CloseAsync();
            return data;
        }

        public async Task<ulong> GettextchannelId(ulong? guildId)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {guildId};";

            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(dataTable);


            if (dataTable.Rows.Count == 0)
            {
                await connection.CloseAsync();
                return 0;
            }
            var channelId = ulong.Parse(dataTable.Rows[0][4].ToString());
            

            await connection.CloseAsync();
            return channelId;
        }

        async public Task<bool> LogKick(DiscordChannel channel, DiscordMember target, DiscordMember admin, string reason)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {channel.GuildId};";
            var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return false;
            }
            await reader.CloseAsync();

            command.CommandText = $"INSERT INTO dbo.Kicked_Users Values({channel.GuildId},{target.Id},'{reason}', '{DateTime.Now}', {admin.Id});";

            await command.ExecuteNonQueryAsync();
            //command.CommandText = $"SELECT * FROM dbo.Kicked_Users ;";
            //DataTable dataTable = new DataTable();
            //SqlDataAdapter adapter = new SqlDataAdapter();
            //adapter.SelectCommand = command;
            //adapter.Fill(dataTable);
            await connection.CloseAsync();
            return true;

        }
        async public Task<bool> LogBan(DiscordChannel channel, DiscordMember target, DiscordMember admin, string reason)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {channel.GuildId};";
            var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return false;
            }
            await reader.CloseAsync();

            command.CommandText = $"INSERT INTO dbo.Banned_Users Values({channel.GuildId},{target.Id},'{reason}', '{DateTime.Now}', {admin.Id});";

            await command.ExecuteNonQueryAsync();
            //command.CommandText = $"SELECT * FROM dbo.Banned_Users ;";
            //DataTable dataTable = new DataTable();
            //SqlDataAdapter adapter = new SqlDataAdapter();
            //adapter.SelectCommand = command;
            //adapter.Fill(dataTable);
            await connection.CloseAsync();
            return true;

        }

        async public Task<bool> LogMute(DiscordChannel channel, DiscordMember target, DiscordMember admin, string reason, long timeMinutes)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {channel.GuildId};";
            var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return false;
            }
            await reader.CloseAsync();

            command.CommandText = $"INSERT INTO dbo.Muted_Users Values({channel.GuildId},{target.Id},'{reason}', '{DateTime.Now}', {admin.Id},{timeMinutes});";

            await command.ExecuteNonQueryAsync();
            //command.CommandText = $"SELECT * FROM dbo.Muted_Users ;";
            //DataTable dataTable = new DataTable();
            //SqlDataAdapter adapter = new SqlDataAdapter();
            //adapter.SelectCommand = command;
            //adapter.Fill(dataTable);
            await connection.CloseAsync();
            return true;
        }

        async public Task<Dictionary<ulong, Dictionary<ulong, int>>> GetExp()
        {
            var result = new Dictionary<ulong, Dictionary<ulong, int>>();
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;



            command.CommandText = $"SELECT * FROM dbo.Guilds ;"; //ДОБАВИТЬ В СЛОВАРЬ ВСЕ ИД СЕРВЕРОВ, ИЗ ТАБЛИЦЫ, И ПОСЛЕ ВСТАВЛЯТЬ ПОЛЬЗОВАТЕЛЕЙ
            DataTable dataTable = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                result[ulong.Parse(row[0].ToString())] = new Dictionary<ulong, int>();
                
            }
            command.CommandText = $"SELECT * FROM dbo.Users;";
            dataTable = new DataTable();
            adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(dataTable);

            foreach (DataRow row in dataTable.Rows)
            {
                result[ulong.Parse(row[0].ToString())][ulong.Parse(row[1].ToString())] = int.Parse(row[3].ToString());
            }
            return result;
        }

        async public Task<bool> InsertUser(DiscordChannel channel, DiscordUser author)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {channel.GuildId};";
            var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return false;
            }
            await reader.CloseAsync();

            command.CommandText = $"INSERT INTO dbo.Users Values({channel.GuildId},{author.Id},0, 0, 'user');";

            await command.ExecuteNonQueryAsync();
            //command.CommandText = $"SELECT * FROM dbo.Banned_Users ;";
            //DataTable dataTable = new DataTable();
            //SqlDataAdapter adapter = new SqlDataAdapter();
            //adapter.SelectCommand = command;
            //adapter.Fill(dataTable);
            await connection.CloseAsync();
            return true;
        }
        
        async public Task<bool> UpdateLevel(DiscordChannel channel, DiscordUser author)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {channel.GuildId};";
            var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return false;
            }
            await reader.CloseAsync();

            command.CommandText = $"UPDATE dbo.Users SET user_level = user_level+1 WHERE guild_id = {channel.GuildId} AND user_id = {author.Id};";

            await command.ExecuteNonQueryAsync();
            //command.CommandText = $"SELECT * FROM dbo.Banned_Users ;";
            //DataTable dataTable = new DataTable();
            //SqlDataAdapter adapter = new SqlDataAdapter();
            //adapter.SelectCommand = command;
            //adapter.Fill(dataTable);
            await connection.CloseAsync();
            return true;
        }
        async public Task<bool> UpdateExp(ulong GuildId, ulong authorId, int exp)
        {
            if (connection == null || connection.State != ConnectionState.Open) await Connect();
            var command = new SqlCommand();
            command.Connection = connection;
            command.CommandText = $"SELECT * FROM dbo.Guilds where guild_id = {GuildId};";
            var reader = await command.ExecuteReaderAsync();
            if (!reader.HasRows)
            {
                await connection.CloseAsync();
                return false;
            }
            await reader.CloseAsync();

            command.CommandText = $"UPDATE dbo.Users SET user_exp = {exp} WHERE guild_id = {GuildId} AND user_id = {authorId};";

            await command.ExecuteNonQueryAsync();
            //command.CommandText = $"SELECT * FROM dbo.Banned_Users ;";
            //DataTable dataTable = new DataTable();
            //SqlDataAdapter adapter = new SqlDataAdapter();
            //adapter.SelectCommand = command;
            //adapter.Fill(dataTable);
            await connection.CloseAsync();
            return true;
        }
    }

}
