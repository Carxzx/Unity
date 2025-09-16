using UnityEngine;

public class ProximitySound : MonoBehaviour
{
    public AudioSource sonido;       // Asigna en el Inspector

    const float distanciaMin = 2f;  // Pegado al objeto
    const float distanciaMax = 20f;   // Más allá de esto no se escucha nada
    public bool atenuar;

    void Update()
    {
        if (!sonido.isPlaying)
        {
            sonido.loop = true;   // Asegúrate de que está en bucle
            sonido.Play();
        }

        float distancia = Vector3.Distance(Player.tf.position, transform.position);

        if (distancia >= distanciaMax)
        {
            sonido.volume = 0f; // Fuera de rango, silencio
        }
        else if (distancia <= distanciaMin)
        {
            sonido.volume = 1f; // Muy cerca, volumen máximo
        }
        else
        {
            // Normalizamos a [0,1], siendo 0 = maxDist, 1 = minDist
            float t = 1f - ((distancia - distanciaMin) / (distanciaMax - distanciaMin));
            
            if(atenuar){
                // Cuadrática suave
                sonido.volume = t * t * 0.5f;
            }else{
                sonido.volume = t * t;
            }
        }
    }
}
