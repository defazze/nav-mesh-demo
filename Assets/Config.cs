using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Config : IComponentData
{
    public float cellSize;
    public int cellWidth;
    public int cellHeight;

    public float3 CellToWorld(int2 cell)
    {
        var transform = new float3(-cellSize * cellWidth / 2, -cellSize * cellHeight / 2, 0);
        var result = new float3(cell.x * cellSize, cell.y * cellSize, 0) + transform;
        return result;
    }
}