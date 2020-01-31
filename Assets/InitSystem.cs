using Unity.Entities;

public class InitSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        var navMesh = EntityManager.CreateEntity(typeof(NavMesh));
        EntityManager.AddBuffer<Cell>(navMesh);
    }

    protected override void OnUpdate()
    {

    }
}