using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class TextBox : MonoBehaviour
{
    const float TiempoEntreCaracteres = 0.015f;

    public TMP_Text Dialogue;
    public TMP_Text Nombre;
    public GameObject triangle;
    int cont;
    public string message;
    float tiempo;
    public bool messageComplete;
    public bool bandera;

    public string personaje;
    public GameObject spriteChavalin;
    public GameObject spriteMinotauro;

    public GameObject caja;
    public GameObject nombre;

    public Sprite cajaNormal;
    public Sprite cajaConPersonaje;

    public AudioSource audio;

    bool alternar;
    
    void Start(){
        Dialogue.text = "";
        cont = 0;
        tiempo = 0f;
        messageComplete = false;
        triangle.SetActive(false);
        bandera = false;
    }

    void OnEnable(){
        spriteChavalin.SetActive(false);
        spriteMinotauro.SetActive(false);
        caja.SetActive(false);
        StartCoroutine(ElegirCaja());
    }

    IEnumerator ElegirCaja(){
        yield return null;
        caja.SetActive(true);
        if(personaje == "Jugador" || personaje == Historia.nombreJugador){
            TextBoxJugador();
        }else if(personaje == "Minotauro"){
            TextBoxMinotauro();
        }else{
            TextBoxNormal();
        }
    }

    void Update(){
        TextBoxHandler(message);
        MostrarPersonaje(personaje);
    }

    //Manejador del Cuadro de Texto
    public void TextBoxHandler(string message){
        //Hasta que el mensaje no esté completado, se imprimen caracteres
        if(!messageComplete && gameObject.activeSelf){
            tiempo += Time.deltaTime;
            if(tiempo > TiempoEntreCaracteres){
                PrintMessage(message);
            }
            if(Input.GetMouseButtonDown(0)){
                cont = message.Length;
            }
        }

        //Si el mensaje se ha completado, y el usuario pulsa el click izquierdo, se quita el Cuadro de Texto
        if(messageComplete && Input.GetMouseButtonDown(0)){
            ReiniciarObject();
            bandera = true;
            //UI_Handler.OcultarUI(gameObject);
            //StartCoroutine(EsperarFrame());
        }
    }

    private IEnumerator EsperarFrame(){
        yield return null;
    }

    //Imprimir mensaje caracter por caracter
    void PrintMessage(string message){
        int numCaracteres = message.Length;
        if (cont <= numCaracteres){
            Dialogue.text = (message.Substring(0, cont));

            // Pausa normal
            float pausa = TiempoEntreCaracteres;

            // Si hay más caracteres, revisa el siguiente
            if (cont > 0) {
                char siguiente = message[cont-1];
                if (siguiente == '.' || siguiente == '?' || siguiente == '!' || siguiente == ':') {
                    pausa = TiempoEntreCaracteres * 20f; // Pausa más larga
                    alternar = true;
                }else if(siguiente == ','){
                    pausa = TiempoEntreCaracteres * 10f; // Pausa más corta
                    alternar = true;
                }
            }
            cont++;

            if(alternar){
                ReproducirDesdeAudioSource(audio);
            }
            alternar = !alternar;

            tiempo = -pausa; // Usar negativo para que Time.deltaTime lo compense en el siguiente frame
        }
        if(cont > numCaracteres){
            messageComplete = true;
            triangle.SetActive(true);
        }
    }

    public void ReproducirDesdeAudioSource(AudioSource original)
    {
        // Crear un GameObject temporal para el AudioSource
        GameObject tempGO = new GameObject("AudioTemporal");
        tempGO.transform.position = original.transform.position; // opcional: posición del sonido
        AudioSource aSource = tempGO.AddComponent<AudioSource>();

        // Copiar el clip y la configuración del original
        aSource.clip = original.clip;
        aSource.volume = 0.25f;
        aSource.pitch = 0.6f;
        aSource.loop = false; // para que no se quede en bucle
        aSource.spatialBlend = original.spatialBlend; // 2D o 3D
        aSource.time = 0.15f;

        aSource.Play();

        // Destruir el GameObject cuando termine de sonar
        Destroy(tempGO, aSource.clip.length - 0.15f);
    }


    public void MoverTextBox(){
        gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y*-1);
    }

    //Devuelve el Cuadro de Texto a sus valores iniciales y lo desactiva
    void ReiniciarObject(){
        Start();
    }

    void MostrarPersonaje(string p){
        if(p == "Jugador" || p == Historia.nombreJugador){
            spriteChavalin.SetActive(true);
            spriteMinotauro.SetActive(false);
        }else if(p == "Minotauro"){
            spriteChavalin.SetActive(false);
            spriteMinotauro.SetActive(true);
        }else{
            spriteChavalin.SetActive(false);
            spriteMinotauro.SetActive(false);
        }
    }

    void TextBoxJugador(){
        //Debug.Log("Jugador");
        caja.transform.localScale = new Vector2(1,1);
        nombre.transform.localPosition = new Vector2(236.5f, nombre.transform.localPosition.y);
        caja.GetComponent<Image>().sprite = cajaConPersonaje;
    }

    void TextBoxMinotauro(){
        //Debug.Log("Minotauro");
        caja.transform.localScale = new Vector2(-1,1);
        caja.GetComponent<Image>().sprite = cajaConPersonaje;
        nombre.transform.localPosition = new Vector2(-236.5f, nombre.transform.localPosition.y);
    }

    void TextBoxNormal(){
        //Debug.Log("Otro");
        caja.GetComponent<Image>().sprite = cajaNormal;
    }
}
