using Ordering;
using UnityEngine;

public class RandomLight : MonoBehaviour
{
    [SerializeField] GameObject Light;
    private void OnTriggerEnter(Collider other)
    {
        int random = UnityEngine.Random.Range(1, 10);
        if (random == 1)
        {
            Light.SetActive(false);
        }
        int randoms = UnityEngine.Random.Range(1, 21);
        if (randoms == 1)
        {
            Terminal.Instance.Broke();
        }
    }

}
