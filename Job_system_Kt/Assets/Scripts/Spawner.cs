using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine.Jobs;

public class ObjectGenerator : MonoBehaviour
{
    [SerializeField] GameObject template;
    [SerializeField] int amount = 700;
    [SerializeField] float circleSize = 12f;
    [SerializeField] float rotationSpeed = 1.8f;
    [SerializeField] float debugDelay = 1.5f;

    TransformAccessArray transArray;
    NativeArray<float> randomAngles;
    NativeArray<float> logStorage;
    float nextDebugTime;
    JobHandle activeJob;

    void Awake()
    {
        transArray = new TransformAccessArray(amount);
        randomAngles = new NativeArray<float>(amount, Allocator.Persistent);
        logStorage = new NativeArray<float>(amount, Allocator.Persistent);

        for (int n = 0; n < amount; n++)
        {
            var clone = Instantiate(template);
            transArray.Add(clone.transform);
            randomAngles[n] = UnityEngine.Random.Range(-3.14159f, 3.14159f);
        }
    }

    void Update()
    {
        activeJob.Complete();

        var orbitTask = new OrbitalTransformJob
        {
            currentTime = Time.time,
            orbitRadius = circleSize,
            angularSpeed = rotationSpeed,
            phaseShifts = randomAngles
        };

        activeJob = orbitTask.Schedule(transArray);
        nextDebugTime += UnityEngine.Time.deltaTime;

        if (nextDebugTime >= debugDelay)
        {
            nextDebugTime = 0f;

            var logTask = new NaturalLogTask
            {
                output = logStorage,
                rngBase = (uint)UnityEngine.Random.Range(100, 999999)
            };

            activeJob = logTask.Schedule(activeJob);
        }
    }

    void LateUpdate() => activeJob.Complete();

    void OnDestroy()
    {
        activeJob.Complete();

        if (transArray.isCreated) transArray.Dispose();
        if (randomAngles.IsCreated) randomAngles.Dispose();
        if (logStorage.IsCreated) logStorage.Dispose();
    }
}