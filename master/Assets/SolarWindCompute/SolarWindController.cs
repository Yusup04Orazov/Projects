using System;
using UnityEngine;

[ExecuteInEditMode]
public class SolarWindController : MonoBehaviour {
    ParticleSystem vectorFieldParticleSystem;
    ParticleSystem.EmissionModule emmission;
    ParticleSystem.Particle[] particles;

    [Range(0,1000)]
    public float lerpMax;

    public float particleCharge;
    public float particleMass;
    public float earthMagneticFieldMagnitude;
    public float earthRadius;

    [SerializeField]
    public GameObject earth;

    public Vector3 earthDipole = new Vector4(0, Mathf.Cos(0.204203522f), Mathf.Sin(0.204203522f));

    public ComputeShader computeShader;
    private ComputeBuffer velocityBuffer;
    private ComputeBuffer positionBuffer;
    private int kernelID;

    private Vector3[] velocityArray;
    private Vector3[] positionArray;

    private const int threadCount = 1024;

    void Update() {
        //Read-in alive particles
        InitializeParticleSystem();
        int aliveParticleCount = vectorFieldParticleSystem.GetParticles(particles);

        if(aliveParticleCount == 0 || vectorFieldParticleSystem.isPaused) {return;}

        //Allocate position and field buffers
        positionBuffer = new ComputeBuffer(aliveParticleCount, sizeof(float) * 3);
        positionArray = new Vector3[aliveParticleCount];

        velocityBuffer = new ComputeBuffer(aliveParticleCount, sizeof(float) * 3);
        velocityArray = new Vector3[aliveParticleCount];

        //Read-in position data to position buffer
        for(int i = 0; i < aliveParticleCount; i++) {
            positionArray[i] = particles[i].position - particles[i].velocity * Time.deltaTime;
            velocityArray[i] = particles[i].velocity;
        }

        positionBuffer.SetData(positionArray);
        velocityBuffer.SetData(velocityArray);

        //Set buffers and dispatch compute shader
        computeShader.SetBuffer(kernelID, "_Position", positionBuffer);
        computeShader.SetBuffer(kernelID, "_Velocity", velocityBuffer);
        computeShader.SetInt("_ParticleCount", aliveParticleCount);

        computeShader.SetFloat("_particleCharge", particleCharge);
        computeShader.SetFloat("_particleMass", particleMass);
        computeShader.SetFloat("_earthMagneticFieldMagnitude", earthMagneticFieldMagnitude);
        computeShader.SetFloat("_earthRadius", earthRadius);
        computeShader.SetVector("_earthDipole", earthDipole);

        computeShader.SetFloat("_initialSpeed", vectorFieldParticleSystem.main.startSpeed.constant);
        computeShader.SetFloat("_dt", Time.deltaTime);

        computeShader.Dispatch(kernelID, (int)Mathf.Ceil(aliveParticleCount/(float)threadCount), 1, 1);

        positionBuffer.GetData(positionArray);
        velocityBuffer.GetData(velocityArray);
  
        for(int i = 0; i < aliveParticleCount; i++) {
            particles[i].position = positionArray[i];
            particles[i].velocity = velocityArray[i];

            if(particles[i].position.magnitude < earthRadius) {particles[i].startLifetime = 0;}

            particles[i].startColor = Remap(0, lerpMax, Color.blue, Color.red,  lerpMax - particles[i].position.magnitude);
        }

        vectorFieldParticleSystem.SetParticles(particles, aliveParticleCount); 

        positionBuffer.Release();
        velocityBuffer.Release();

    }  

    void InitializeParticleSystem() {
        if (vectorFieldParticleSystem == null) {
            vectorFieldParticleSystem = GetComponent<ParticleSystem>();
            emmission = vectorFieldParticleSystem.emission;
        }

        if (particles == null || particles.Length < vectorFieldParticleSystem.main.maxParticles) {
            particles = new ParticleSystem.Particle[vectorFieldParticleSystem.main.maxParticles];
        }
    }

    private void OnEnable() {
        kernelID = computeShader.FindKernel("CSMain");
    }

    private void OnDisable() {
        if(velocityBuffer != null) {
            velocityBuffer.Release();
            velocityBuffer = null;
        }
    }

    bool checkIfOutsideBoundry(Vector3 radialVector) {
        if(radialVector.magnitude > 6) {return true;}
        return false;
    }

    float Remap(float iMin, float iMax, float oMin, float oMax, float v) {
                    float t = Mathf.InverseLerp(iMin, iMax, v);
                    return Mathf.Lerp(oMin, oMax, t);
    }

    Color Remap(float iMin, float iMax, Color oMin, Color oMax, float v) {
                    float t = Mathf.InverseLerp(iMin, iMax, v);
                    return Color.Lerp(oMin, oMax, t);
    }

    Vector3 NormalizePosition(Vector3 position)
    {
        float magnitude = position.magnitude;
        if (magnitude > 0)
        {
            return position / magnitude;
        }
        return position;
    }

    Vector3 NormalizeVelocity(Vector3 velocity)
    {
        float magnitude = velocity.magnitude;
        if (magnitude > 0)
        {
            return velocity / magnitude;
        }
        return velocity;
    }
}