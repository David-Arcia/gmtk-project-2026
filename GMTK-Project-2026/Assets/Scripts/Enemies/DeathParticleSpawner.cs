using UnityEngine;

public class DeathParticleSpawner : MonoBehaviour
{
    [SerializeField]
    Color normalDeathColor;
    [SerializeField]
    Color dashResetColor;
    [SerializeField]
    GameObject particleEmitterPrefab;

    public void RequestParticleEffect(Vector2 location, bool isDashReset)
    {
        GameObject particleInstance = Instantiate(particleEmitterPrefab, location, Quaternion.identity);
        ParticleSystem particles = particleInstance.transform.Find("Particles").GetComponent<ParticleSystem>();
        ParticleSystem.MainModule mainModule = particles.main;
        if (isDashReset)
        {
            mainModule.startColor = dashResetColor;
        } else
        {
            mainModule.startColor = normalDeathColor;
        }
    }
}
