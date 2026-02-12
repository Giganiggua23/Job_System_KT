using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

[BurstCompile]
public struct OrbitalTransformJob : IJobParallelForTransform
{
    public float currentTime;
    public float orbitRadius;
    public float angularSpeed;

    [ReadOnly]
    public NativeArray<float> phaseShifts;

    public void Execute(int idx, TransformAccess trans)
    {
        float phi = currentTime * angularSpeed + phaseShifts[idx];

        float xPos = math.sin(phi) * orbitRadius;
        float zPos = math.cos(phi) * orbitRadius;

        trans.position = new float3(xPos, 0f, zPos);
    }
}