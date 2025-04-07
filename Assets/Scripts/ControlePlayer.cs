using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ControlePlayer : MonoBehaviour
{
    [SerializeField] float aceleracao,forcaPulo,velocidadeMaxima;
    [SerializeField] LayerMask mascaraDeLayers;
    [SerializeField] Image barraPulo;
    [SerializeField] TextMeshProUGUI textoPontos, vidas;

    int items, quantidadeVidas;
    

    bool noChao = false;
    bool jumping=false;
    
    Rigidbody2D rb;

    InputAction move;
    InputAction jump;
    private void Start()
    {
        items = 0;
        quantidadeVidas = 3;
        move = InputSystem.actions.FindAction("Move");
        jump = InputSystem.actions.FindAction("Jump");
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Regenera());
        Pontos();
    }

    IEnumerator Regenera()
    {        
        barraPulo.fillAmount += 0.001f;
        yield return new WaitForSeconds(0.001f);
        StartCoroutine(Regenera());
    }

    private void Update()
    {
        if (jump.WasPressedThisFrame() && noChao && barraPulo.fillAmount > 0)
        {
            jumping = true;
            barraPulo.fillAmount -= 0.1f;
        }
        Pontos();
        Vidas();
    }
    private void FixedUpdate()
    {
        Vector2 direcao = move.ReadValue<Vector2>();
        if (direcao != Vector2.zero)
        {
            rb.AddForce(Vector2.right * direcao.x * aceleracao, ForceMode2D.Force);
            if (rb.linearVelocity.magnitude > velocidadeMaxima)
            {
                rb.linearVelocityX = velocidadeMaxima * direcao.x;
            }
        }
        else
        {
            rb.AddForce(new Vector2(rb.linearVelocityX * -aceleracao, 0), ForceMode2D.Force);
        }
        if (jumping)
        {
            rb.AddForce(Vector2.up * forcaPulo, ForceMode2D.Impulse);
            jumping=false;
        }

       Vector2 baseObjeto = new Vector2(transform.position.x, 
            GetComponent<BoxCollider2D>().bounds.min.y);

        noChao = Physics2D.OverlapCircle(baseObjeto, 0.1f, mascaraDeLayers);     
    }
    void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            items++;            
            Destroy(collision.gameObject);
        }
    }
    public void Pontos()
    {
        textoPontos.text = "Itens: " + items.ToString("000");
        if (items % 3 == 0)
        {
            quantidadeVidas++;
        }
    }

    public void Vidas()
    {
        vidas.text = "Vidas: " + quantidadeVidas.ToString();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
     //   noChao = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
     //   noChao = false;
    }
}
