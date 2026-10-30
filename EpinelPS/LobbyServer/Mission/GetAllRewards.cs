namespace EpinelPS.LobbyServer.Mission;

[GameRequest("/mission/getrewarded/all")]
public class GetAllRewards : LobbyMessage
{
    protected override async Task HandleAsync()
    {
        ReqGetRewardedData req = await ReadData<ReqGetRewardedData>();
        User user = GetUser();

        // Apply a pending daily/weekly rollover first so the completed lists
        // below describe the current period instead of the previous one.
        user.ResetDataIfNeeded();

        ResGetRewardedData response = new();

        response.AchievementIds.Add(user.CompletedAchievements);
        response.WeeklyIds.Add(user.WeeklyResetableData.CompletedWeeklyMissions);
        response.DailyIds.Add(user.ResetableData.CompletedDailyMissions);

        await WriteDataAsync(response);
    }
}
