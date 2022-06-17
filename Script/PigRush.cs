using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigRush : MonoBehaviour
{
    private Animator anim;
    public float speed;
    public bool isRight;
    private AnimatorOverrideController[] overrideController;
    // Start is called before the first frame update
    private void Awake()
    {
        overrideController = new AnimatorOverrideController[2];
        anim = GetComponent<Animator>();
        overrideController[0] = Resources.Load<AnimatorOverrideController>("PigAnimator/Left");
        overrideController[1] = Resources.Load<AnimatorOverrideController>("PigAnimator/Right");
      
    }
    void Start()
    {
        float y = Random.Range(-4.5f, 2f);
        anim.runtimeAnimatorController = isRight ? overrideController[1] : overrideController[0];
        if (!isRight)
        {
            transform.position = new Vector3(8.14999962f, y, 0);
        }
        else
        {
            anim.runtimeAnimatorController = overrideController[1];
            transform.position = new Vector3(-8.14999962f, y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {   
        if(!isRight)
            transform.Translate( -transform.right *speed*Time.deltaTime);
        else
            transform.Translate(transform.right * speed * Time.deltaTime);
        if(transform.position.x>10f||transform.position.x<-10f)
        {
            Destroy(gameObject);
        }
    }
}
