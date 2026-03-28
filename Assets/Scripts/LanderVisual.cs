using UnityEngine;

public class LanderVisual : MonoBehaviour
{
    [SerializeField] private ParticleSystem leftThrusterParticlelSystem;
    [SerializeField] private ParticleSystem middleThrusterParticlelSystem;
    [SerializeField] private ParticleSystem rightThrusterParticlelSystem;

    private Lander lander;

    private void Awake()
    {
        lander = GetComponent<Lander>();

        lander.OnUpForce += Lander_OnUpForce;
        lander.OnLeftForce += Lander_OnLeftForce;
        lander.OnRightForce += Lander_OnRightForce;
        lander.OnBeforeForce += Lander_OnBeforeForce;

        SetEnableThrusterParticleSystem(leftThrusterParticlelSystem, false);
        SetEnableThrusterParticleSystem(middleThrusterParticlelSystem, false);
        SetEnableThrusterParticleSystem(rightThrusterParticlelSystem, false);

    }

    private void Lander_OnBeforeForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(leftThrusterParticlelSystem, false);
        SetEnableThrusterParticleSystem(middleThrusterParticlelSystem, false);
        SetEnableThrusterParticleSystem(rightThrusterParticlelSystem, false);
    }

    private void Lander_OnRightForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(leftThrusterParticlelSystem, true);
        SetEnableThrusterParticleSystem(middleThrusterParticlelSystem, false);
        SetEnableThrusterParticleSystem(rightThrusterParticlelSystem, false);
    }

    private void Lander_OnLeftForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(leftThrusterParticlelSystem, false);
        SetEnableThrusterParticleSystem(middleThrusterParticlelSystem, false);
        SetEnableThrusterParticleSystem(rightThrusterParticlelSystem, true);
    }

    private void Lander_OnUpForce(object sender, System.EventArgs e)
    {
        SetEnableThrusterParticleSystem(leftThrusterParticlelSystem, true);
        SetEnableThrusterParticleSystem(middleThrusterParticlelSystem, true);
        SetEnableThrusterParticleSystem(rightThrusterParticlelSystem, true);
    }

    private void SetEnableThrusterParticleSystem(ParticleSystem particleSystem, bool enable)
    {
        ParticleSystem.EmissionModule emissionModule = particleSystem.emission;
        emissionModule.enabled = enable; 
    }

}
