# Discans
Discord bot to notify when manga chapter is released in english, based on MangaUpdates website. You can invite the last version of Discans to your server [using this link](https://discordapp.com/api/oauth2/authorize?client_id=556213286383648769&permissions=154624&scope=bot).

## Commands

| Command | Description |
| ------- | ----------- |
| info | Discans introduce itself and says Github page link to user see all commands available. | 
| channel | Set the default channel to receive user alerts or server alerts. |
| server-alert [link] | Creates an alert to @everyone when a new manga chapter be released in that Discord server. **Only an Admin can use that command.** | 
| server-alert-remove [link] | Delete a server alert already registered to @everyone in that Discord server. **Only an Admin can use that command.** |
| server-alert-remove-all | Remove manga's alerts in the Discord Server. It can be User Alerts or Server Alerts. **Only an Admin can use that command** and **has a confirmation message.** |
| server-alert-list | List all alerts configurated to @everyone in this that Discord Server. | 
| server-alert-list-all | ist all alerts configurated to @everyone and @users in this that Discord Server. | 
| user-alert [link] [user(s)] | Create an alert to @user(s) when a new manga chapter be released. **Only an Admin can create alert to other users than not you.** | 
| user-alert-remove [link] [user(s)] | Remove manga's alerts assigned to @user(s). **Only an Admin can remove alerts from other users than not you.** | 
| user-alert-remove [user(s)] | Removes all alerts assigned to @user(s). **Only an Admin can remove alerts from other users than not you.** |
| user-alert-list | List all alerts configurated to @users in this that Discord Server. | 
| private-alert [link] | Creates an alert to send you a Direct Message when a new manga chapter be released. |
| private-alert-remove [link] | Remove manga's alerts assigned to the user in Direct Message. |
| private-alert-list | List all alerts configurated to send as Direct Message for that user. |

### Commands usage examples
```
discans info
discans channel
discans server-alert https://www.mangaupdates.com/series.html?id=88
discans server-alert-remove https://www.mangaupdates.com/series.html?id=88
discans server-alert-remove-all
discans server-alert-list
discans server-alert-list-all
discans user-alert https://www.mangaupdates.com/series.html?id=88 @user#1
discans user-alert https://www.mangaupdates.com/series.html?id=88 @user#1 @user#2 @user#3
discans user-alert-remove https://www.mangaupdates.com/series.html?id=88 @user#1
discans user-alert-remove https://www.mangaupdates.com/series.html?id=88 @user#1 @user#2 @user#3
discans user-alert-remove @user#1 
discans user-alert-remove @user#1 @user#2 @user#3
discans user-alert-list
discans private-alert https://www.mangaupdates.com/series.html?id=88
discans private-alert-remove https://www.mangaupdates.com/series.html?id=88
discans private-alert-list
```

## Requirements & How to build

* MySQL server and his connection string;
* Download and install [.Net Core 2.2 SDK](https://dotnet.microsoft.com/download);
* Replace `config.json` variables with your own ones to run in a **non production environment**;
* Register all `config.json` variables as Environment Variables to run in a **production environment**;

A production environment is defined as `ASPNETCORE_ENVIRONMENT` == `production`, case insensitively as Environment Variable. 

`Discans` Project runs the Discord bot to interact with user messages, and `Discans.WebJob` is a cron job who checks when there is a new manga chapter released. 

## Pending Developments

- [x] Alert @Everyone
- [x] Alert @User
- [ ] Alert @Role
- [x] Alert @User by Direct Message
