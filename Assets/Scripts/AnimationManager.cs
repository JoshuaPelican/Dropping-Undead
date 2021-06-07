using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    //Updates the animator setting a float parameter
    public void UpdateAnimation(string floatToSet, float value)
    {
        if(anim.GetFloat(floatToSet) != value)
        {
            anim.SetFloat(floatToSet, value);
        }
    }

    //Updates the animator setting a boolean value
    public void UpdateAnimation(string boolToSet, bool value)
    {
        if(anim.GetBool(boolToSet) != value)
        {
            anim.SetBool(boolToSet, value);
        }
    }
}
