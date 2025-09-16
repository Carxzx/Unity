using UnityEngine;
using System.Collections;

public class Esqueleto : MonoBehaviour
{
    const float distancePerFrame = 2f;
    private Vector3 posicionInicial;
    public bool prioridad;
    private bool reposicionar;

    private bool persiguiendo = false;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    public Animator anim;
    
    public Sprite[] lado;
    public Sprite[] espalda;
    public Sprite[] frente;
    public bool walking = false;
    public int fotograma;
    Vector3 direccion;

    Vector3 inicio;
    Vector3 fin;

    const float RaycastDistance = 0.5f;

    public int vida = 5;
    private bool retroceder;

    int layerEsqueleto;
    int layerMask;
    int layerTrigger;
    int layerIgnoreRaycast;
    int layerHerramienta;
    int layerObjeto;

    const float intervalo = 0.35f; // 0,35 segundos entre pasos
    private float timer = 0f;
    public AudioSource paso;

    private float radioMax = 7f;
    private float volumenMax = 0.1f;

    const float difMax = 0.01f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        paso = GetComponent<AudioSource>();
        posicionInicial = transform.position;
        reposicionar = false;
        layerEsqueleto = LayerMask.NameToLayer("Esqueleto");
        layerTrigger = LayerMask.NameToLayer("EsqueletoTrigger");
        layerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        layerHerramienta = LayerMask.NameToLayer("Herramienta");

        layerMask = ~( (1 << layerEsqueleto) | (1 << layerIgnoreRaycast) | (1 << layerTrigger) | (1 << layerHerramienta)); // Ignora ambas layers
    }

    void FixedUpdate()
    {
        if(walking){
            if(!reposicionar && !retroceder){
                Collider2D propio = GetComponent<Collider2D>();

                RaycastHit2D[] UP = LanzarRaycast(Vector2.up);
                RaycastHit2D[] DOWN = LanzarRaycast(Vector2.down);
                RaycastHit2D[] LEFT = LanzarRaycast(Vector2.left);
                RaycastHit2D[] RIGHT = LanzarRaycast(Vector2.right);

                Vector3 correccion = Vector3.zero;
                if(direccion == Vector3.left){
                    ResolverColisionLateral(LEFT[0], LEFT[1], LEFT[2], Vector3.left);
                }else if(direccion == Vector3.right){
                    ResolverColisionLateral(RIGHT[0], RIGHT[1], RIGHT[2], Vector3.right);
                }else if(direccion == Vector3.up){
                    ResolverColisionLateral(UP[0], UP[1], UP[2], Vector3.up);
                }else if(direccion == Vector3.down){
                    ResolverColisionLateral(DOWN[0], DOWN[1], DOWN[2], Vector3.down);
                }
            }
        }
    }

    void ResolverColisionLateral(RaycastHit2D esquina1, RaycastHit2D centro, RaycastHit2D esquina2, Vector2 direccionPrincipal)
    {
        // Esto será el resultado: hacia donde mover al Player
        Vector3 correccion = Vector3.zero;

        bool e1 = esquina1.collider != null; // AI (o equivalente)
        bool c  = centro.collider  != null;  // C
        bool e2 = esquina2.collider != null; // AD (o equivalente)

        if(!e1 && !c && !e2) return;

        Vector2 diferencia = Player.tf.position - transform.position;
        
        // Caso 1: esquina1 libre, centro y esquina2 bloqueados
        if (direccionPrincipal == Vector2.up || direccionPrincipal == Vector2.down){
            if(diferencia.x > difMax){
                prioridad = false;
                direccion = Vector2.right;
            }else if(diferencia.x < -difMax){
                prioridad = false;
                direccion = Vector2.left;
            }else{
                if ((!e1 && c && e2) || (!e1 && !c && e2)){
                    StartCoroutine(CorregirMovimiento(Vector2.left, direccionPrincipal));
                }else{
                    StartCoroutine(CorregirMovimiento(Vector2.right, direccionPrincipal));
                }
            }
        }else{
            if(diferencia.y > difMax){
                prioridad = true;
                direccion = Vector2.up;
            }else if(diferencia.y < -difMax){
                prioridad = true;
                direccion = Vector2.down;
            }else{
                if ((!e1 && c && e2) || (!e1 && !c && e2)){
                    StartCoroutine(CorregirMovimiento(Vector2.up, direccionPrincipal));
                }else{
                    StartCoroutine(CorregirMovimiento(Vector2.down, direccionPrincipal));
                }
            }
        }
    }

    IEnumerator CorregirMovimiento(Vector3 dir, Vector2 direccionPrincipal)
    {
        System.Random rnd = new System.Random();
        Vector3 inicio = transform.position;
        Vector3 destino = inicio + dir * 100;

        walking = true;
        reposicionar = true;
        direccion = dir;

        RaycastHit2D[] hit = LanzarRaycast(direccionPrincipal);

        while (hit[0].collider != null || hit[1].collider != null || hit[2].collider != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, destino, distancePerFrame * Time.deltaTime);

            hit = LanzarRaycast(direccionPrincipal);

            anim.SetBool("Walk",walking);
            if(walking){
                AsignarSprite(direccion,fotograma);
            }
            //Debug.Log("Corrigiendo movimiento hacia: " + dir);

            yield return null;
        }

        walking = false;
        reposicionar = false;
    }

    RaycastHit2D[] LanzarRaycast(Vector2 direccion)
    {
        Vector2 halfSize = new Vector2(0.25f, 0.375f);
        Vector2 offset   = new Vector2(0f, 0.375f);

        // Esquinas y centros
        Vector2 bottomLeft  = (Vector2)transform.position + offset + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = (Vector2)transform.position + offset + new Vector2( halfSize.x, -halfSize.y);
        Vector2 topLeft     = (Vector2)transform.position + offset + new Vector2(-halfSize.x,  halfSize.y);
        Vector2 topRight    = (Vector2)transform.position + offset + new Vector2( halfSize.x,  halfSize.y);

        Vector2 centerTop    = (Vector2)transform.position + offset + new Vector2(0f,  halfSize.y);
        Vector2 centerBottom = (Vector2)transform.position + offset + new Vector2(0f, -halfSize.y);
        Vector2 centerLeft   = (Vector2)transform.position + offset + new Vector2(-halfSize.x, 0f);
        Vector2 centerRight  = (Vector2)transform.position + offset + new Vector2( halfSize.x, 0f);

        // Posiciones iniciales de los rayos
        Vector2 start1 = Vector2.zero;
        Vector2 start2 = Vector2.zero;
        Vector2 startCenter = Vector2.zero;

        if (direccion == Vector2.up){
            start1 = topLeft;
            start2 = topRight;
            startCenter = centerTop;
        }else if (direccion == Vector2.down){
            start1 = bottomLeft;
            start2 = bottomRight;
            startCenter = centerBottom;
        }else if (direccion == Vector2.left){
            start1 = topLeft;
            start2 = bottomLeft;
            startCenter = centerLeft;
        }else if (direccion == Vector2.right){
            start1 = topRight;
            start2 = bottomRight;
            startCenter = centerRight;
        }

        // Lanzar raycasts
        RaycastHit2D hit1 = Physics2D.Raycast(start1, direccion, RaycastDistance, layerMask);
        RaycastHit2D hit2 = Physics2D.Raycast(start2, direccion, RaycastDistance, layerMask);
        RaycastHit2D hitCenter = Physics2D.Raycast(startCenter, direccion, RaycastDistance, layerMask);

        // Debug en build
        Debug.DrawRay(start1, direccion * RaycastDistance, Color.red, 1f);
        Debug.DrawRay(start2, direccion * RaycastDistance, Color.red, 1f);
        Debug.DrawRay(startCenter, direccion * RaycastDistance, Color.red, 1f);

        //Debug.Log("Raycast " + direccion + " desde " + start1 + " golpea: " + (hit1.collider ? hit1.collider.gameObject.name : "null"));
        //Debug.Log("Raycast " + direccion + " desde " + start2 + " golpea: " + (hit2.collider ? hit2.collider.gameObject.name : "null"));
        //Debug.Log("Raycast " + direccion + " desde " + startCenter + " golpea: " + (hitCenter.collider ? hitCenter.collider.gameObject.name : "null"));

        return new RaycastHit2D[] { hit1, hitCenter, hit2 };
    }

    void Update()
    {
        if(Player.BloquearMovimiento) return;

        if(!reposicionar && !retroceder){
            Vector3 dif = Player.tf.position - transform.position;

            if(Player.tf == null){
                //Debug.LogError("Player.tf no está asignado");
                return;
            }
            if(persiguiendo && Mathf.Abs(dif.x) < difMax && Mathf.Abs(dif.y) < difMax){
                walking = false;
                rb.linearVelocity = Vector2.zero;
            }else if (persiguiendo){
                Vector2 inicio = transform.position;
                Vector2 destino = Player.tf.position;
                direccion = destino - inicio;
                Vector2 movimiento = Vector2.zero;

                // Primero mueve en Y hasta estar alineado, luego en X
                if(prioridad){
                    if (Mathf.Abs(direccion.y) > difMax)
                    {
                        movimiento.y = Mathf.Sign(direccion.y) * distancePerFrame * Time.deltaTime;
                        if(direccion.y < 0){
                            direccion = Vector2.down;
                        }else{
                            direccion = Vector2.up;
                        }
                    }
                    else if (Mathf.Abs(direccion.x) > difMax)
                    {
                        prioridad = false;
                        movimiento.x = Mathf.Sign(direccion.x) * distancePerFrame * Time.deltaTime;
                        if(direccion.x < 0){
                            direccion = Vector2.left;
                        }else{
                            direccion = Vector2.right;
                        }
                    }

                    transform.position = inicio + movimiento;
                    walking = true;
                }else{
                    if (Mathf.Abs(direccion.x) > difMax)
                    {
                        movimiento.x = Mathf.Sign(direccion.x) * distancePerFrame * Time.deltaTime;
                        if(direccion.x < 0){
                            direccion = Vector2.left;
                        }else{
                            direccion = Vector2.right;
                        }
                    }
                    else if (Mathf.Abs(direccion.y) > difMax)
                    {
                        prioridad = true;
                        movimiento.y = Mathf.Sign(direccion.y) * distancePerFrame * Time.deltaTime;
                        if(direccion.y < 0){
                            direccion = Vector2.down;
                        }else{
                            direccion = Vector2.up;
                        }
                    }

                    transform.position = inicio + movimiento;
                    walking = true;
                }
            }
            else if (!persiguiendo && transform.position != posicionInicial)
            {
                Vector2 inicio = transform.position;
                Vector2 destino = posicionInicial;
                direccion = destino - inicio;
                Vector2 movimiento = Vector2.zero;

                // Primero mueve en Y hasta estar alineado, luego en X
                if(prioridad){
                    if (Mathf.Abs(direccion.y) > difMax)
                    {
                        movimiento.y = Mathf.Sign(direccion.y) * distancePerFrame * Time.deltaTime;
                        if(direccion.y < 0){
                            direccion = Vector2.down;
                        }else{
                            direccion = Vector2.up;
                        }

                    }else if (Mathf.Abs(direccion.x) > difMax)
                    {
                        movimiento.x = Mathf.Sign(direccion.x) * distancePerFrame * Time.deltaTime;
                        if(direccion.x < 0){
                            direccion = Vector2.left;
                        }else{
                            direccion = Vector2.right;
                        }
                    }

                    transform.position = inicio + movimiento;
                    walking = true;
                }else{
                    if (Mathf.Abs(direccion.x) > difMax)
                    {
                        movimiento.x = Mathf.Sign(direccion.x) * distancePerFrame * Time.deltaTime;
                        if(direccion.x < 0){
                            direccion = Vector2.left;
                        }else{
                            direccion = Vector2.right;
                        }
                    }
                    else if (Mathf.Abs(direccion.y) > difMax)
                    {
                        movimiento.y = Mathf.Sign(direccion.y) * distancePerFrame * Time.deltaTime;
                        if(direccion.y < 0){
                            direccion = Vector2.down;
                        }else{
                            direccion = Vector2.up;
                        }
                    }

                    transform.position = inicio + movimiento;
                    walking = true;
                }
            }
            else
            {
                rb.linearVelocity = Vector2.zero;
                walking = false;
            }
            direccion = ComprobarDireccion();
            anim.SetBool("Walk",walking);
            if(walking){
                AsignarSprite(direccion,fotograma);
            }else{
                AsignarSprite(direccion,0);
            }
        }

        if (walking){
            timer -= Time.deltaTime;
            if (timer <= 0f){
                // Calcular distancia al oyente
                float distancia = Vector3.Distance(Player.tf.position, transform.position);

                if (distancia <= radioMax)
                {
                    // Volumen cuadrático inverso
                    float t = 1f - (distancia / radioMax); // 0 cuando lejos, 1 cuando cerca
                    float volumen = Mathf.Clamp01(t * t) * volumenMax;

                    paso.volume = volumen;
                    paso.Play();
                }

                timer = intervalo;
            }
        }else{
            timer = 0f;
            if(paso.isPlaying) paso.Stop();
        }
    }

    void AsignarSprite(Vector2 dir, int fot){
        if (dir == Vector2.up) {
            sr.sprite = espalda[fot];
        } else if (dir == Vector2.down) {
            sr.sprite = frente[fot];
        } else if (dir == Vector2.right || dir == Vector2.left) {
            sr.flipX = (dir == Vector2.right) ? true : false;
            sr.sprite = lado[fot];
        }
        //Debug.Log(dir);
    }

    Vector2 ComprobarDireccion(){
        if(direccion.y > 0.01f){ //Movimiento hacia arriba
            return Vector2.up;
        }else if(direccion.y < -0.01f){ //Movimiento hacia debajo
            return Vector2.down;
        }else if(direccion.x > 0.01f){ //Movimiento hacia la derecha

            return Vector2.right;
        }else if(direccion.x < -0.01f){ //Movimiento hacia la izquierda
            return Vector2.left;
        }
        return direccion;
    }

    public void DestruirEsqueleto(){
        //Dropear hueso
        GameObject hueso = Resources.Load<GameObject>("Prefabs/ObjetosInventario/Hueso");
        GameObject instancia = Instantiate(hueso, gameObject.transform.position, Quaternion.identity);

        instancia.GetComponent<Objeto>().cantidad = Random.Range(1, 4);
        Destroy(gameObject);
    }

    public void Retroceder(Vector2 direccion){
        StartCoroutine(_Retroceder(direccion));
    }

    private IEnumerator _Retroceder(Vector2 direccion){
        Vector2 inicio = transform.position;
        Vector2 destino = inicio + direccion;

        float tiempo = 0.2f;
        float elapsed = 0f;

        retroceder = true;

        transform.position = Vector2.Lerp(inicio, destino, 0.01f);

        Color colorInicial = Color.white;
        Color colorFinal = Color.red;

        while (elapsed < tiempo) {
            float t = elapsed / tiempo;
            transform.position = Vector2.Lerp(inicio, destino, t);
            sr.color = Color.Lerp(colorInicial, colorFinal, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        sr.color = Color.white;

        retroceder = false;

        transform.position = destino;
    }

    // Estos métodos serán llamados desde el trigger del mapa
    public void EmpezarPersecucion() => persiguiendo = true;
    public void PararPersecucion() => persiguiendo = false;
}