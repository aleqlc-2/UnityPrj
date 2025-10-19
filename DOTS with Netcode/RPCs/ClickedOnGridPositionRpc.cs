using Unity.Entities;

public struct ClickedOnGridPositionRpc : IComponentData
{
    public int x;
    public int y;
    public PlayerType playerType;
}
