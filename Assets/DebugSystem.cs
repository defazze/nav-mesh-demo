using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//[DisableAutoCreation]
public class DebugSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NavMesh>();
        RequireSingletonForUpdate<Config>();
    }
    protected override void OnUpdate()
    {
        var navMesh = GetSingletonEntity<NavMesh>();
        var cells = EntityManager.GetBuffer<Cell>(navMesh).ToNativeArray(Allocator.Temp);

        var config = GetSingleton<Config>();
        foreach (var cell in cells)
        {
            var pos = config.CellToWorld(cell.value);

            Debug.DrawLine(pos, pos + new float3(config.cellSize, config.cellSize, 0), Color.yellow);
        }
    }
}