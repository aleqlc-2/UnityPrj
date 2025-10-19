using Unity.NetCode;

public struct GameWinRpc : IRpcCommand
{
    public PlayerType winningPlayerType;
}
