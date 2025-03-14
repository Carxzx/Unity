using UnityEngine;
using TMPro;

public class TextBox : MonoBehaviour
{
    const float TiempoEntreCaracteres = 0.05f;

    public TMP_Text Dialogue;
    public GameObject triangle;
    int cont;
    string message;
    float tiempo;
    bool messageComplete;
    
    void Start(){
        Dialogue.text = "";
        gameObject.SetActive(false);
        triangle.SetActive(false);
        cont = 0;
        tiempo = 0f;
        messageComplete = false;
    }

    
    void Update(){

        //Example Text
        message = "Que pasa picha aquí Joaquín, con la nueva Gillete Labs con barra exfoliante, que te deja la piel como la primera pasada. Estoy guapo eh?";
        
        TextBoxHandler(message);
    }

    //Manejador del Cuadro de Texto
    void TextBoxHandler(string message){
        //Hasta que el mensaje no esté completado, se imprimen caracteres
        if(gameObject.activeSelf && !messageComplete){
            Player.CanMove = false;
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
            gameObject.SetActive(false);
            triangle.SetActive(false);
        }
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
}
