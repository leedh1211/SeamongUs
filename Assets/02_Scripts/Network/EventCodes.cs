public static class EventCodes
{
    //-----------------------MissionCode-------------------------//
    public const byte MissionsAssigned = 1;
    public const byte MissionCompleted = 2;
    public const byte MissionsAssignedCompleted = 3;
    public const byte MissionCompletedUIRefresh = 4;
    
    
    //-----------------------PlayerCode-------------------------//
    public const byte PlayerSpawn = 100;
    public const byte PlayerJump = 101;
    public const byte PlayerKill = 102;
    public const byte PlayerDied = 103;
    public const byte PlayerAttacked = 104;
    
    //----------------------GameManageCode----------------------//
    public const byte ChangeState = 200;
    
}