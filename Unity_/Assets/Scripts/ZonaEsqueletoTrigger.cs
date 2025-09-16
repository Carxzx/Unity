using UnityEngine;

public class ZonaEsqueletoTrigger : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Esqueleto esq in FindObjectsOfType<Esqueleto>())
            {
                esq.EmpezarPersecucion();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Esqueleto esq in FindObjectsOfType<Esqueleto>())
            {
                esq.PararPersecucion();
            }
        }
    }
}