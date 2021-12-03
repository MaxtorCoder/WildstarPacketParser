namespace WildstarPacketParser.Constants;

public enum RemoveReason
{
    Kicked          = 0x01,
    Disconnected    = 0x02,
    Left            = 0x03,
    Disband         = 0x04,
    RemovedByServer = 0x05,
    VoteKicked      = 0x06,
    RemovedForPvP   = 0x07
}
