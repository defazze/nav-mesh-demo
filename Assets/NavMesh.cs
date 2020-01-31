using Unity.Entities;
using Unity.Mathematics;

public struct NavMesh : IComponentData
{

}

public struct Cell : IBufferElementData
{
    public int2 value;
}