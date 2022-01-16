using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
 
    private bool hit;

    private float direction;
    [SerializeField] public float speed;

    private BoxCollider2D boxcollider;

    private Animator anim;
    private Movement move;
  
    private void Awake()
    {
        //References
        anim = GetComponent<Animator>();
        boxcollider = GetComponent<BoxCollider2D>();
        move = GetComponent<Movement>();
    }


    void Update()
    {
        if (hit) return;
        float movementspeed = speed * Time.deltaTime * direction;
        transform.Translate(movementspeed, 0, 0);
    }
    //Exploding the fireball when it has collided with something
    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxcollider.enabled = false;
        anim.SetTrigger("Fireball_explode");
      //  Destroy(gameObject);

    }
    //Setting the direction of the fireball
    public void SetDirection(float _direction)
    {
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxcollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX = -localScaleX;
        }

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }
    //Deativating the fireball
    public void Deactivate()
    {
        Destroy(gameObject);
    }
    
}
