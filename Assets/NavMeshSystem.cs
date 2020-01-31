using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[DisableAutoCreation]
public class NavMeshSystem : ComponentSystem
{
    private BuildPhysicsWorld physicsWorldSystem;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NavMesh>();
        RequireSingletonForUpdate<Config>();

        physicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        var config = GetSingleton<Config>();
        var navMeshEntity = GetSingletonEntity<NavMesh>();
        var cells = EntityManager.GetBuffer<Cell>(navMeshEntity);
        cells.Clear();

        for (int x = 0; x < config.cellWidth; x++)
        {
            for (int y = 0; y < config.cellHeight; y++)
            {
                var position = config.CellToWorld(new int2(x, y));

                var start = position - new float3(0, 0, -0.1f);
                var end = position - new float3(0, 0, 0.1f);

                var collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
                var raycastInput = new RaycastInput
                {
                    Start = start,
                    End = end,
                    Filter = CollisionFilter.Default
                };

                if (collisionWorld.CastRay(raycastInput))
                {
                    cells.Add(new Cell { value = new int2(x, y) });
                }
            }
        }
    }
}