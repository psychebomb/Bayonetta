using UnityEngine;

public class RandomAnimSelect : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        animator = this.gameObject.GetComponent<Animator>();
        int randomInt = Random.Range(0, 2);
        if (animator) animator.SetInteger("rand", randomInt);
    }

}

