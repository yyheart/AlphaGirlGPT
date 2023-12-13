using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AlphaGirl_Anima : MonoBehaviour
{
    public static AlphaGirl_Anima instance;
    Animator animator_AlphaGirl;
    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator_AlphaGirl = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            AlphaGirlRotate();
        }
    }
    float rotateSpeed = 1f;
    float x;
    /// <summary>
    /// AlphaGirl左右旋转
    /// </summary>
    public void AlphaGirlRotate()
    {

        x = Input.GetAxis("Mouse X");
        gameObject.transform.Rotate(Vector3.down, x * rotateSpeed);
    }

    public void SetAlphaGirlAnimatorState(AudioClip clip)
    {
        StartCoroutine(AlphaGirlTalk(clip));
    }

    IEnumerator AlphaGirlTalk(AudioClip clip)
    {
        animator_AlphaGirl.SetBool("IsTalk", true);
        yield return new WaitForSeconds(clip.length);
        animator_AlphaGirl.SetBool("IsTalk", false);

    }

    public void StopAlphaGirlAnima()
    {
        animator_AlphaGirl.SetBool("IsTalk", false);

    }
}

