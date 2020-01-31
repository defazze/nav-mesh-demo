using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateAfter(typeof(StepPhysicsWorld))]
[UpdateAfter(typeof(EndFramePhysicsSystem))]
public class NavMeshJobSystem : JobComponentSystem
{
    private BuildPhysicsWorld physicsWorldSystem;

    //[BurstCompile]
    public struct NavMeshJob : IJobParallelFor
    {
        [NativeDisableContainerSafetyRestriction]
        public NativeQueue<int2>.ParallelWriter queue;
        public Config config;
        [ReadOnly] public PhysicsWorld physicsWorld;

        public void Execute(int index)
        {
            var cell = new int2(index % config.cellWidth, index / config.cellWidth);

            var position = config.CellToWorld(new int2(cell.x, cell.y));

            var start = position - new float3(0, 0, -0.1f);
            var end = position - new float3(0, 0, 0.1f);

            var collisionWorld = physicsWorld.CollisionWorld;
            var raycastInput = new RaycastInput
            {
                Start = start,
                End = end,
                Filter = CollisionFilter.Default
            };

            if (collisionWorld.CastRay(raycastInput))
            {
                queue.Enqueue(new int2(cell.x, cell.y));
            }

        }
    }

    private struct UnionJob : IJobForEach_B<Cell>
    {
        [NativeDisableContainerSafetyRestriction] public NativeQueue<int2> queue;
        public void Execute(DynamicBuffer<Cell> buffer)
        {
            buffer.Clear();
            while (queue.TryDequeue(out int2 position))
            {
                buffer.Add(new Cell { value = position });
            }
        }
    }

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NavMesh>();
        RequireSingletonForUpdate<Config>();

        physicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var queue = new NativeQueue<int2>(Allocator.TempJob);
        var writer = queue.AsParallelWriter();

        var config = GetSingleton<Config>();
        var job = new NavMeshJob
        {
            config = config,
            physicsWorld = physicsWorldSystem.PhysicsWorld,
            queue = writer
        };

        var unionJob = new UnionJob
        {
            queue = queue
        };

        var physicHandle = JobHandle.CombineDependencies(physicsWorldSystem.FinalJobHandle, inputDeps);
        var handle = job.Schedule(config.cellWidth * config.cellHeight, 64, physicHandle);
        var resultHandle = unionJob.Schedule(this, handle);
        queue.Dispose();

        //resultHandle.Complete();
        return resultHandle;
    }
}