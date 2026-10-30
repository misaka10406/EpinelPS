using EpinelPS.Database;

namespace EpinelPS.LobbyServer.Inventory;

/// <summary>
/// Handles /inventory/equipment/unlockoption.
/// The client calls this when the player cancels a lock on one awakening option slot.
/// Clears both the permanent lock and the disposable lock for the given slot.
/// </summary>
[GameRequest("/inventory/equipment/unlockoption")]
public class UnlockOption : LobbyMessage
{
    protected override async Task HandleAsync()
    {
        ReqAwakeningUnlockOption req = await ReadData<ReqAwakeningUnlockOption>();
        User user = GetUser();

        ResAwakeningUnlockOption response = new();

        if (req.Isn <= 0 || req.Slot is < 1 or > 3)
        {
            await WriteDataAsync(response);
            return;
        }

        // Apply to every entry with this Isn (confirmed entry and pending preview entry)
        // so the unlocked state is consistent regardless of confirmation state.
        foreach (EquipmentAwakeningData awakening in user.EquipmentAwakenings.Where(x => x.Isn == req.Isn).ToList())
        {
            ClearSlotLock(awakening.Option, req.Slot);
        }

        JsonDb.Save();

        await WriteDataAsync(response);
    }

    private static void ClearSlotLock(NetEquipmentAwakeningOption option, int slot)
    {
        switch (slot)
        {
            case 1:
                option.Option1Lock = false;
                option.IsOption1DisposableLock = false;
                break;
            case 2:
                option.Option2Lock = false;
                option.IsOption2DisposableLock = false;
                break;
            case 3:
                option.Option3Lock = false;
                option.IsOption3DisposableLock = false;
                break;
        }
    }
}
