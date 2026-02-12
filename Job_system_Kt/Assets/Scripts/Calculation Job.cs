using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
public struct NaturalLogTask : IJob
{
    public NativeArray<float> output;
    public uint rngBase;

    public void Execute()
    {
        var prng = new Random(rngBase);

        for (int position = 0; position < output.Length; position++)
        {
            float arbitrary = prng.NextFloat(0.5f, 200f);
            output[position] = math.log10(arbitrary) * 2.302585f; // ln(x) = log10(x) * ln(10)
        }
    }
}