using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasMenu : MonoBehaviour
{
    public Image imagenNegro;         // Arrastra la imagen negra desde la UI
    public MusicaFondo musicaFondo;   // Arrastra el script MusicaFondo (que tiene el AudioSource de la música)
    const float duracion = 2f;       // Duración de la transición
    private bool iniciado = false;
    public AudioSource sonidoMenu;

    public GameObject Controles;
    public GameObject Creditos;
    public GameObject FondoControles;

    public GameObject botonJugar;
    public GameObject botonControles;
    public GameObject botonCreditos;

    void Start(){
        if (!iniciado){
            iniciado = true;
            StartCoroutine(FadeIn());
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            if(Controles.activeSelf){
                CerrarControles();
            }
            if(Creditos != null && Creditos.activeSelf){
                CerrarCreditos();
            }
        }
    }

    public void IniciarTransicion(){
        if (!iniciado){
            iniciado = true;
            StartCoroutine(FadeOut());
            PlayAndDestroy(sonidoMenu);
        }
    }

    private IEnumerator FadeOut(){
        float tiempo = 0f;

        // Guardamos valores iniciales
        Color colorInicial = imagenNegro.color;
        float alphaInicial = 0f;   // Transparencia mínima (invisible)
        float alphaFinal = 1f;     // Transparencia máxima (totalmente negro)

        float volumenInicial = musicaFondo.GetComponent<AudioSource>().volume;
        float volumenFinal = 0f;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion; // Normalizado (0 → 1)

            // Interpolamos la alpha de la imagen
            float nuevoAlpha = Mathf.Lerp(alphaInicial, alphaFinal, t);
            imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, nuevoAlpha);

            // Interpolamos el volumen de la música
            musicaFondo.GetComponent<AudioSource>().volume = Mathf.Lerp(volumenInicial, volumenFinal, t);

            yield return null; // Esperar al siguiente frame
        }

        // Aseguramos estado final
        imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alphaFinal);
        musicaFondo.GetComponent<AudioSource>().volume = volumenFinal;


        iniciado = false;
        if(SceneManager.GetActiveScene().name == "CarroScene"){
            SceneManager.LoadScene("SampleScene");
        }else if(SceneManager.GetActiveScene().name == "SampleScene"){
            SceneManager.LoadScene("CarroScene");
        }
    }

    private IEnumerator FadeIn(){
        float tiempo = 0f;

        // Guardamos valores iniciales
        Color colorInicial = imagenNegro.color;
        float alphaInicial = 1f;   // Comienza negro (totalmente opaco)
        float alphaFinal = 0f;     // Termina transparente (invisible)

        float volumenInicial = 0f; // Música en silencio al inicio
        float volumenFinal = musicaFondo.GetComponent<AudioSource>().volume; // Volumen normal de la música

        // Reiniciamos la alpha y el volumen antes de empezar
        imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alphaInicial);
        musicaFondo.GetComponent<AudioSource>().volume = volumenInicial;

        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            float t = tiempo / duracion; // Normalizado (0 → 1)

            // Interpolamos la alpha de la imagen
            float nuevoAlpha = Mathf.Lerp(alphaInicial, alphaFinal, t);
            imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, nuevoAlpha);

            // Interpolamos el volumen de la música
            musicaFondo.GetComponent<AudioSource>().volume = Mathf.Lerp(volumenInicial, volumenFinal, t);

            yield return null; // Esperar al siguiente frame
        }

        // Aseguramos estado final
        imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alphaFinal);
        musicaFondo.GetComponent<AudioSource>().volume = volumenFinal;

        iniciado = false;
    }

    
    void PlayAndDestroy(AudioSource sourceToPlay)
    {
        // Crear un GameObject temporal
        GameObject tempGO = new GameObject("AudioTemporal");
        tempGO.transform.position = transform.position;

        // Clonar el AudioSource del original
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = sourceToPlay.clip;
        aSource.volume = sourceToPlay.volume;
        aSource.pitch = sourceToPlay.pitch;
        aSource.loop = false; // Nos aseguramos de que no se repita
        aSource.spatialBlend = sourceToPlay.spatialBlend;
        aSource.playOnAwake = false;

        // Reproducir el clip
        aSource.Play();

        // Destruir el GameObject temporal después de que termine
        Destroy(tempGO, aSource.clip.length + 0.5f);
    }

    private void ActivarBotones(){
        botonJugar.SetActive(true);
        botonControles.SetActive(true);
        botonCreditos.SetActive(true);
    }

    private void DesactivarBotones(){
        botonJugar.SetActive(false);
        botonControles.SetActive(false);
        botonCreditos.SetActive(false);
    }

    public void ActivarControles(){
        if(!iniciado){
            DesactivarBotones();
            Controles.SetActive(true);
            FondoControles.SetActive(true);
            PlayAndDestroy(sonidoMenu);
        }
    }

    public void CerrarControles(){
        ActivarBotones();
        Controles.SetActive(false);
        FondoControles.SetActive(false);
        PlayAndDestroy(sonidoMenu);
    }

    public void ActivarCreditos(){
        if(!iniciado){
            DesactivarBotones();
            Creditos.SetActive(true);
            FondoControles.SetActive(true);
            PlayAndDestroy(sonidoMenu);
        }
    }

    public void CerrarCreditos(){
        ActivarBotones();
        Creditos.SetActive(false);
        FondoControles.SetActive(false);
        PlayAndDestroy(sonidoMenu);
    }

    public void Salir(){
        if (!iniciado){
            iniciado = true;
            StartCoroutine(FadeOut());
            PlayAndDestroy(sonidoMenu);
        }
    }

    

    void OnDestroy(){
        // Si instancias AudioTemporal en algún momento, asegúrate de borrarlo
        GameObject obj = GameObject.Find("AudioTemporal");
        if (obj != null)
        {
            Destroy(obj);
        }
    }

    
}
