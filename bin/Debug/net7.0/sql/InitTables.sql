--drop table Banned_Users;
--drop table Kicked_Users;
--drop table Muted_Users;
--drop table Users;
--drop table Guilds;

IF OBJECT_ID(N'dbo.Guilds', N'U') IS NULL
CREATE TABLE Guilds (
    guild_id NUMERIC(20,0) NOT NULL,
    guild_init_date VARCHAR(100),
    guild_type VARCHAR(100),
    guild_cat_id NUMERIC(20,0),
    guild_log_id NUMERIC(20,0),
    PRIMARY KEY("guild_id")
);

IF OBJECT_ID(N'dbo.Users', N'U') IS NULL
CREATE TABLE Users (
    guild_id NUMERIC(20,0) NOT NULL,
    user_id NUMERIC(20,0) NOT NULL,
    user_level INT NOT NULL DEFAULT 0,
    user_exp INT NOT NULL DEFAULT 0,
    user_role VARCHAR(255) NOT NULL DEFAULT 'user',
    foreign key(guild_id) references dbo.Guilds(guild_id) on update cascade on delete cascade,
    PRIMARY KEY("guild_id","user_id")
);

IF OBJECT_ID(N'dbo.Kicked_Users', N'U') IS NULL
CREATE TABLE Kicked_Users (
    kick_id NUMERIC(20,0) NOT NULL IDENTITY,
    guild_id NUMERIC(20,0) NOT NULL,
    user_id NUMERIC(20,0) NOT NULL,
    kick_reason VARCHAR(255) DEFAULT '',
    kick_date VARCHAR(100),
    admin_id NUMERIC(20,0),
    foreign key(guild_id) references dbo.Guilds(guild_id) on update cascade on delete cascade,
    PRIMARY KEY("kick_id")
);

IF OBJECT_ID(N'dbo.Banned_Users', N'U') IS NULL
CREATE TABLE Banned_Users (
    ban_id NUMERIC(20,0) NOT NULL IDENTITY,
    guild_id NUMERIC(20,0) NOT NULL,
    user_id NUMERIC(20,0) NOT NULL,
    ban_reason VARCHAR(255) DEFAULT '',
    ban_date VARCHAR(100),
    admin_id NUMERIC(20,0),
    foreign key(guild_id) references dbo.Guilds(guild_id) on update cascade on delete cascade,
    PRIMARY KEY("ban_id")
);

IF OBJECT_ID(N'dbo.Muted_Users', N'U') IS NULL
CREATE TABLE Muted_Users (
    mute_id NUMERIC(20,0) NOT NULL IDENTITY,
    guild_id NUMERIC(20,0) NOT NULL,
    user_id NUMERIC(20,0) NOT NULL,
    mute_reason VARCHAR(255) DEFAULT '',
    mute_date VARCHAR(100),
    admin_id NUMERIC(20,0),
    time_in_minutes NUMERIC(20,0),
    foreign key(guild_id) references dbo.Guilds(guild_id) on update cascade on delete cascade,
    PRIMARY KEY("mute_id")
);

