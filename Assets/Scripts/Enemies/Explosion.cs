using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public int damage = 6;
    
    private float _lifetime = .1f;

    private void OnEnable()
    {
        StartCoroutine(Remove());
    }

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IHealth>().ChangeHealth(-damage);
    }
    
    private IEnumerator Remove()
    {
        yield return new WaitForSeconds(_lifetime);
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        StopCoroutine(Remove());
    }
}