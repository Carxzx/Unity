using UnityEngine;
using TMPro;
using System.Collections;

public class TextBox : MonoBehaviour
{
    const float TiempoEntreCaracteres = 0.03f;

    public TMP_Text Dialogue;
    public TMP_Text Nombre;
    public GameObject triangle;
    int cont;
    public string message;
    float tiempo;
    public bool messageComplete;
    public bool bandera;
    
    void Start(){
        Dialogue.text = "";
        cont = 0;
        tiempo = 0f;
        messageComplete = false;
        triangle.SetActive(false);
        bandera = false;
    }

    
    void Update(){

        //Example Text
        //message = "Que pasa picha aquí Joaquín, con la nueva Gillete Labs con barra exfoliante, que te deja la piel como la primera pasada. Estoy guapo eh?";

        TextBoxHandler(message);
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
        if(cont <= numCaracteres){
            Dialogue.text = (message.Substring(0, cont));
            cont++;
            tiempo = 0f;
        }else{
            messageComplete = true;
            triangle.SetActive(true);
        }
    }


    public void MoverTextBox(){
        gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y*-1);
    }

    //Devuelve el Cuadro de Texto a sus valores iniciales y lo desactiva
    void ReiniciarObject(){
        Start();
    }
}
