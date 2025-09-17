using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Historia : MonoBehaviour
{
    [System.Serializable]
    public class Dialogo{
        public int id;
        public string personaje;
        public string[] lineas;
        public bool finalizado_dialogo;
    }

    [System.Serializable]
    public class BloquesDialogo{
        public Dialogo[] dialogo;
    }

    public BloquesDialogo CargarDialogo(string nombreArchivo){
        TextAsset json = Resources.Load<TextAsset>("Dialogs/" + nombreArchivo);
        if(json == null){
            Debug.LogError("No se encontró el archivo de diálogo: " + nombreArchivo);
            return null;
        }

        BloquesDialogo bloques = JsonUtility.FromJson<BloquesDialogo>(json.text);

        return bloques;
    }

    UI_Handler UI_Handler;
    TextBox TextBox;
    Player Player;
    public InventoryData InventoryData;
    public PlayerCamera PlayerCamera;
    public TMP_InputField inputField;
    static public string nombreJugador;

    public Sprite Flor1;
    public Sprite Flor2;
    public Sprite Flor3;
    public Sprite Madera;
    public Sprite Piedra;
    public Sprite Hueso;

    private bool SawSkeletons = false;

    public Image imagenNegro;

    void Start(){
        UI_Handler = FindFirstObjectByType<UI_Handler>();
        TextBox = UI_Handler.TextBox.GetComponent<TextBox>();
        Player = FindFirstObjectByType<Player>();
        nombreJugador = "";
    }

    void ActivarEvento(string nombre){
        Transform t = null;
        foreach (Transform tr in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (tr.name == nombre) {
                t = tr;
                break;
            }
        }
        if (t != null) {
            t.gameObject.SetActive(true);
        }
    }

    void DesactivarEvento(string nombre){
        Transform t = null;
        foreach (Transform tr in Resources.FindObjectsOfTypeAll<Transform>()) {
            if (tr.name == nombre) {
                t = tr;
                break;
            }
        }
        if (t != null) {
            t.gameObject.SetActive(false);
        }
    }

    private IEnumerator MostrarDialogo(Dialogo dialogo){
        TextBox.gameObject.SetActive(true);
        int i = 0;
        TextBox.Nombre.text = dialogo.personaje;
        TextBox.personaje = dialogo.personaje;
        dialogo.finalizado_dialogo = false;
        while(i < dialogo.lineas.Length){
            TextBox.message = dialogo.lineas[i];
            TextBox.bandera = false;
            while(!TextBox.bandera){
                yield return null;
            }
            i++;
        }
        dialogo.finalizado_dialogo = true;
        TextBox.personaje = "";
        TextBox.gameObject.SetActive(false);
    }

    void ActualizarNombreJSON(BloquesDialogo bloques){
        foreach (Dialogo d in bloques.dialogo){
            d.personaje = d.personaje.Replace("{nombre}", nombreJugador);
            for (int i = 0; i < d.lineas.Length; i++){
                d.lineas[i] = d.lineas[i].Replace("{nombre}", nombreJugador);
            }
        }
    }

    public void _GirarseHaciaArriba(){
        //El personaje mira hacia arriba sino lo está
        Player.SR.sprite = Player.vSpriteEspalda[0];
    }

    public void _Prueba(){
        StartCoroutine(Prueba());
    }

    private IEnumerator Prueba(){
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Personajes/Minotauro_NPC");
        Vector2 pos = new Vector3(Player.tf.position.x, Player.tf.position.y-15,0);
        GameObject instancia = Instantiate(prefab,pos,Quaternion.identity);
        NPC_Minotauro minotauro = instancia.GetComponent<NPC_Minotauro>();

        Vector2 inicio = pos;
        Vector2 fin = new Vector2(inicio.x, Player.tf.position.y - 2);
        float distancePerFrame = 1; //0,01f

        minotauro.Moverse(inicio,fin,distancePerFrame);

        //Esperar 3,5segundos
        yield return new WaitForSeconds(4f); //Esperar hasta que termine de moverse

        BloquesDialogo bloques = CargarDialogo("Prueba");
        int numDialogo = 0;
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public void _AcabaDeLlegar(){
        StartCoroutine(AcabaDeLlegar());
    }

    private IEnumerator AcabaDeLlegar(){
        yield return new WaitForSeconds(0.5f);

        float distancePerFrame = 3;
        
        Player.Moverse(Player.tf.position,Player.tf.position + Vector3.down * 7, distancePerFrame);

        while(Player.moverse){
            yield return null;
        }

        Player.Girarse(Vector2.down);

        //Breve pausa tras dejar de moverse
        yield return new WaitForSeconds(0.5f);

        //Empieza el dialogo
        BloquesDialogo bloques = CargarDialogo("AcabaDeLlegar");
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Icons.MostrarIcono(Player.gameObject,"pensativo",1f));

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        EventTrigger trigger = Player.GetComponent<EventTrigger>();
        trigger.evento.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        trigger.evento.RemoveAllListeners();
        trigger.evento.AddListener(_Morir);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public void _BosquePrimeraInteraccion(){
        StartCoroutine(BosquePrimeraInteraccion());
    }

    private IEnumerator BosquePrimeraInteraccion(){
        NPC_Minotauro minotauro = GameObject.Find("MinotauroBosque").GetComponent<NPC_Minotauro>();

        //El personaje mira hacia debajo sino lo está
        Player.SR.sprite = Player.vSpriteFrente[0];

        //Exclamacion del minotauro por 2 segundos
        StartCoroutine(Icons.MostrarIcono(minotauro.gameObject,"exclamacion",1f));
        yield return new WaitForSeconds(1f);

        //Girarse hacia arriba
        minotauro.Girarse(Vector2.up);

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        Vector2 inicio = minotauro.transform.position;
        Vector2 fin = new Vector2(Player.tf.position.x, inicio.y);
        float distancePerFrame = 3; //0,01f
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x, Player.tf.position.y-2.5f);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        //Breve pausa tras dejar de moverse
        yield return new WaitForSeconds(0.2f);

        //Empieza el dialogo
        BloquesDialogo bloques = CargarDialogo("BosquePrimeraInteraccion");
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 2
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        //duda del minotauro por 1 segundo
        StartCoroutine(Icons.MostrarIcono(minotauro.gameObject,"duda",1f));
        yield return new WaitForSeconds(1.5f);

        //Dialogo id = 3
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);


        //Dialogo id = 4
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }

        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 5
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        //Breve pausa
        yield return new WaitForSeconds(0.5f);


        //Dialogo id = 6
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        inputField.gameObject.SetActive(true);

        while(!Input.GetKeyDown(KeyCode.Return) || inputField.text.Length == 0){
            yield return null;
        }

        nombreJugador = inputField.text;

        inputField.gameObject.SetActive(false);

        yield return null;

        ActualizarNombreJSON(bloques);

        yield return null;

        //Dialogo id = 7
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        
        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        //Se empieza a mover de nuevo
        inicio = minotauro.transform.position;
        fin = new Vector2(-999.5f, inicio.y);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x, inicio.y-3f);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        //Breve pausa tras dejar de moverse
        yield return new WaitForSeconds(0.2f);


        //Dialogo id = 8
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }

        bloques.dialogo[numDialogo].finalizado_dialogo = false;
        numDialogo++;

        yield return null;

        //Se gira hacia arriba
        minotauro.Girarse(Vector2.up);

        //Breve pausa
        yield return new WaitForSeconds(0.2f);

        //Dialogo id = 9
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        //Empezar a moverse de nuevo
        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x, 986f);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }


        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x+1f, inicio.y);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }
        

        inicio = minotauro.transform.position;
        fin = new Vector2(inicio.x, 984f);
        minotauro.Moverse(inicio,fin,distancePerFrame);

        while(minotauro.walking){
            yield return null;
        }

        Destroy(minotauro.gameObject);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public void _PrimeraMision(){
        StartCoroutine(PrimeraMision());
    }

    public IEnumerator PrimeraMision(){
        NPC_Minotauro minotauro = GameObject.Find("Minotauro_NPC").GetComponent<NPC_Minotauro>();

        yield return new WaitForSeconds(0.5f);

        BloquesDialogo bloques = CargarDialogo("Primera_Mision");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 2
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 3
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 4
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 5
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 6
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 7
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        Vector2 inicio = Player.tf.position;
        Vector2 fin = new Vector2(Player.tf.position.x+0.5f, inicio.y);
        float distancePerFrame = 3; //0,01f
        Player.Moverse(inicio,fin,distancePerFrame);

        yield return new WaitForSeconds(0.5f);

        //ACTIVAR EVENTO
        ActivarEvento("ComprobarPrimeraMision");

        EventTrigger trigger = minotauro.GetComponent<EventTrigger>();
        trigger.evento.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        trigger.evento.RemoveAllListeners();
        trigger.evento.AddListener(_ComprobarPrimeraMision);

        DesactivarEvento("PrimeraMision");

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
        InventoryData.DarItem(2, "Prefabs/ObjetosInventario/Hacha");
    }

    public void _ComprobarPrimeraMision(){
        int flores = ObtenerFlores();
        int madera = ObtenerMadera();

        if(flores >= 20 && madera >= 32){
            StartCoroutine(SegundaMision());
            DesactivarEvento("ComprobarPrimeraMision");
        }else{
            StartCoroutine(PrimeraMisionIncompleta());
        }
    }

    int ObtenerFlores(){
        int totalFlores = 0;
        for (int i = 0; i < InventoryData.numSlots; i++) {
            Objeto obj = InventoryData.vSlots[i].objeto;
            Sprite sprite = obj.GetComponent<Image>() == null ? obj.GetComponent<SpriteRenderer>().sprite : obj.GetComponent<Image>().sprite;
            if (sprite == Flor1 || sprite == Flor2 || sprite == Flor3) {
                totalFlores += obj.cantidad;
            }
        }
        Debug.Log("Total flores: " + totalFlores);
        return totalFlores;
    }

    int ObtenerMadera(){
        int totalMadera = 0;
        for (int i = 0; i < InventoryData.numSlots; i++) {
            Objeto obj = InventoryData.vSlots[i].objeto;
            Sprite sprite = obj.GetComponent<Image>() == null ? obj.GetComponent<SpriteRenderer>().sprite : obj.GetComponent<Image>().sprite;
            if (sprite == Madera) {
                totalMadera += obj.cantidad;
            }
        }
        Debug.Log("Total madera: " + totalMadera);
        return totalMadera;
    }

    public IEnumerator PrimeraMisionIncompleta(){
        NPC_Minotauro minotauro = GameObject.Find("Minotauro_NPC").GetComponent<NPC_Minotauro>();

        yield return new WaitForSeconds(0.5f);

        BloquesDialogo bloques = CargarDialogo("PrimeraMisionIncompleta");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        Vector2 inicio = Player.tf.position;
        Vector2 fin = new Vector2(Player.tf.position.x+0.5f, inicio.y);
        float distancePerFrame = 3; //0,01f
        Player.Moverse(inicio,fin,distancePerFrame);

        yield return new WaitForSeconds(0.5f);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public IEnumerator SegundaMision(){
        NPC_Minotauro minotauro = GameObject.Find("Minotauro_NPC").GetComponent<NPC_Minotauro>();

        StartCoroutine(Icons.MostrarIcono(minotauro.gameObject,"smile",1f));
        yield return new WaitForSeconds(1.5f);


        BloquesDialogo bloques = CargarDialogo("Segunda_Mision");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Icons.MostrarIcono(minotauro.gameObject,"pensativo",1f));
        yield return new WaitForSeconds(1.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 2
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 3
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 4
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 5
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        Vector2 inicio = Player.tf.position;
        Vector2 fin = new Vector2(Player.tf.position.x+0.5f, inicio.y);
        float distancePerFrame = 3; //0,01f
        Player.Moverse(inicio,fin,distancePerFrame);

        yield return new WaitForSeconds(0.5f);

        //ACTIVAR EVENTO
        ActivarEvento("ComprobarSegundaMision");

        EventTrigger trigger = minotauro.GetComponent<EventTrigger>();
        trigger.evento.RemoveAllListeners();
        trigger.evento.AddListener(_ComprobarSegundaMision);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
        InventoryData.DarItem(3, "Prefabs/ObjetosInventario/Pico");
    }

    public void _ComprobarSegundaMision(){
        if(SawSkeletons){
            StartCoroutine(TerceraMision());
            DesactivarEvento("ComprobarSegundaMision");
        }else{
            StartCoroutine(SegundaMisionIncompleta());
        }
    }

    public IEnumerator SegundaMisionIncompleta(){
        NPC_Minotauro minotauro = GameObject.Find("Minotauro_NPC").GetComponent<NPC_Minotauro>();

        yield return new WaitForSeconds(0.5f);

        BloquesDialogo bloques = CargarDialogo("SegundaMisionIncompleta");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        Vector2 inicio = Player.tf.position;
        Vector2 fin = new Vector2(Player.tf.position.x+0.5f, inicio.y);
        float distancePerFrame = 3; //0,01f
        Player.Moverse(inicio,fin,distancePerFrame);

        yield return new WaitForSeconds(0.5f);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public IEnumerator TerceraMision(){
        NPC_Minotauro minotauro = GameObject.Find("Minotauro_NPC").GetComponent<NPC_Minotauro>();

        yield return new WaitForSeconds(0.5f);

        BloquesDialogo bloques = CargarDialogo("Tercera_Mision");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 2
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 3
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Icons.MostrarIcono(Player.gameObject,"duda",1f));
        yield return new WaitForSeconds(1.5f);

        //Dialogo id = 4
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 5
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        Vector2 inicio = Player.tf.position;
        Vector2 fin = new Vector2(Player.tf.position.x+0.5f, inicio.y);
        float distancePerFrame = 3; //0,01f
        Player.Moverse(inicio,fin,distancePerFrame);

        yield return new WaitForSeconds(0.5f);

        //ACTIVAR EVENTO
        ActivarEvento("ComprobarTerceraMision");

        EventTrigger trigger = minotauro.GetComponent<EventTrigger>();
        trigger.evento.RemoveAllListeners();
        trigger.evento.AddListener(_ComprobarTerceraMision);

        //DESACTIVAR EVENTO
        DesactivarEvento("AvisarMinotauro");

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
        InventoryData.DarItem(4, "Prefabs/ObjetosInventario/Espada");
    }

    public void _ComprobarTerceraMision(){
        int piedras = ObtenerPiedras();
        int huesos = ObtenerHuesos();

        if(piedras >= 32 && huesos >= 1){
            StartCoroutine(Finalizar());
            DesactivarEvento("ComprobarTerceraMision");
        }else{
            StartCoroutine(TerceraMisionIncompleta());
        }
    }

    int ObtenerPiedras(){
        int totalPiedras = 0;
        for (int i = 0; i < InventoryData.numSlots; i++) {
            Objeto obj = InventoryData.vSlots[i].objeto;
            Sprite sprite = obj.GetComponent<Image>() == null ? obj.GetComponent<SpriteRenderer>().sprite : obj.GetComponent<Image>().sprite;
            if (sprite == Piedra) {
                totalPiedras += obj.cantidad;
            }
        }
        Debug.Log("Total piedras: " + totalPiedras);
        return totalPiedras;
    }

    int ObtenerHuesos(){
        int totalHuesos = 0;
        for (int i = 0; i < InventoryData.numSlots; i++) {
            Objeto obj = InventoryData.vSlots[i].objeto;
            Sprite sprite = obj.GetComponent<Image>() == null ? obj.GetComponent<SpriteRenderer>().sprite : obj.GetComponent<Image>().sprite;
            if (sprite == Hueso) {
                totalHuesos += obj.cantidad;
            }
        }
        Debug.Log("Total huesos: " + totalHuesos);
        return totalHuesos;
    }

    public IEnumerator TerceraMisionIncompleta(){
        NPC_Minotauro minotauro = GameObject.Find("Minotauro_NPC").GetComponent<NPC_Minotauro>();

        yield return new WaitForSeconds(0.5f);

        BloquesDialogo bloques = CargarDialogo("TerceraMisionIncompleta");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 2
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        Vector2 inicio = Player.tf.position;
        Vector2 fin = new Vector2(Player.tf.position.x+0.5f, inicio.y);
        float distancePerFrame = 3; //0,01f
        Player.Moverse(inicio,fin,distancePerFrame);

        yield return new WaitForSeconds(0.5f);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public IEnumerator Finalizar(){
        NPC_Minotauro minotauro = GameObject.Find("Minotauro_NPC").GetComponent<NPC_Minotauro>();

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Icons.MostrarIcono(minotauro.gameObject,"smile",1f));
        yield return new WaitForSeconds(1.5f);

        BloquesDialogo bloques = CargarDialogo("Finalizar");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Icons.MostrarIcono(Player.gameObject,"smile",1f));
        yield return new WaitForSeconds(1.5f);

        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public void _Esqueletos(){
        StartCoroutine(Esqueletos());
    }

    public IEnumerator Esqueletos(){
        SawSkeletons = true;
        PlayerCamera.BloquearCamara = true;

        Vector3 inicioC = Camera.main.transform.position;
        Vector3 nuevaPosicion = new Vector3(inicioC.x, inicioC.y - 10f, inicioC.z);
        //MOVIMIENTO DE CAMARA
        yield return StartCoroutine(MoverCamara(inicioC, nuevaPosicion, 2f)); // 2segundos

        //Se para la cámara 2segundos
        yield return new WaitForSeconds(2f);

        //MOVIMIENTO DE CAMARA A SU POSICION ORIGINAL
        yield return StartCoroutine(MoverCamara(nuevaPosicion, inicioC, 2f)); // 2segundos

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Icons.MostrarIcono(Player.gameObject,"duda",1f));
        yield return new WaitForSeconds(1.5f);


        BloquesDialogo bloques = CargarDialogo("AvisarMinotauro");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        Vector2 inicio = Player.tf.position;
        Vector2 fin = new Vector2(Player.tf.position.x, inicio.y+0.5f);
        float distancePerFrame = 3; //0,01f
        Player.Moverse(inicio,fin,distancePerFrame);

        PlayerCamera.BloquearCamara = false;

        yield return new WaitForSeconds(0.5f);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    private IEnumerator MoverCamara(Vector3 inicio, Vector3 fin, float duracion){
        Camera cam = Camera.main;
        float tiempo = 0f;
        while (tiempo < duracion){
            cam.transform.position = Vector3.Lerp(inicio, fin, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }
        cam.transform.position = fin; // Asegura que termine en la posición final
    }

    public void _CartelSala1(){
        StartCoroutine(CartelSala1());
    }

    private IEnumerator CartelSala1(){
        BloquesDialogo bloques = CargarDialogo("CartelSala1");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public void _CartelSala3(){
        StartCoroutine(CartelSala3());
    }

    private IEnumerator CartelSala3(){
        BloquesDialogo bloques = CargarDialogo("CartelSala3");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public void _CartelSala4(){
        StartCoroutine(CartelSala4());
    }

    private IEnumerator CartelSala4(){
        BloquesDialogo bloques = CargarDialogo("CartelSala4");
        ActualizarNombreJSON(bloques);
        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    public void _Morir(){
        StartCoroutine(Morir());
    }

    private IEnumerator Morir(){
        yield return StartCoroutine(FadeOut());
        yield return new WaitForSeconds(0.05f);
        Player.tf.position = new Vector2(-985.5f,899.5f);
        Player.Girarse(Vector2.down);
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(FadeIn());

        yield return new WaitForSeconds(0.5f);

        StartCoroutine(Icons.MostrarIcono(Player.gameObject,"duda",1f));
        yield return new WaitForSeconds(1.5f);

        BloquesDialogo bloques = CargarDialogo("Morir");
        ActualizarNombreJSON(bloques);

        int numDialogo = 0;

        //Dialogo id = 0
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        numDialogo++;
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        //Dialogo id = 1
        StartCoroutine(MostrarDialogo(bloques.dialogo[numDialogo]));

        while(!bloques.dialogo[numDialogo].finalizado_dialogo){
            yield return null;
        }
        bloques.dialogo[numDialogo].finalizado_dialogo = false;

        yield return new WaitForSeconds(0.5f);

        UI_Handler.EventOff(); // Se termina el evento, se quita toda la UI y me puedo mover
    }

    private IEnumerator FadeOut(){
        float tiempo = 0f;
        float duracion = 1f;

        // Guardamos valores iniciales
        Color colorInicial = imagenNegro.color;
        float alphaInicial = 0f;   // Transparencia mínima (invisible)
        float alphaFinal = 1f;     // Transparencia máxima (totalmente negro)

        while (tiempo < duracion){
            tiempo += Time.deltaTime;
            float t = tiempo / duracion; // Normalizado (0 → 1)

            // Interpolamos la alpha de la imagen
            float nuevoAlpha = Mathf.Lerp(alphaInicial, alphaFinal, t);
            imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, nuevoAlpha);

            yield return null; // Esperar al siguiente frame
        }

        // Aseguramos estado final
        imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alphaFinal);
    }

    private IEnumerator FadeIn(){
        float tiempo = 0f;
        float duracion = 1f;

        // Guardamos valores iniciales
        Color colorInicial = imagenNegro.color;
        float alphaInicial = 1f;   // Comienza negro (totalmente opaco)
        float alphaFinal = 0f;     // Termina transparente (invisible)

        // Reiniciamos la alpha y el volumen antes de empezar
        imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alphaInicial);

        while (tiempo < duracion){
            tiempo += Time.deltaTime;
            float t = tiempo / duracion; // Normalizado (0 → 1)

            // Interpolamos la alpha de la imagen
            float nuevoAlpha = Mathf.Lerp(alphaInicial, alphaFinal, t);
            imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, nuevoAlpha);

            yield return null; // Esperar al siguiente frame
        }

        // Aseguramos estado final
        imagenNegro.color = new Color(colorInicial.r, colorInicial.g, colorInicial.b, alphaFinal);
    }
}