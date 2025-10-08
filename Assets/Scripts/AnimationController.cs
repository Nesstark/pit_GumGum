using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
public InputActionProperty grabAction;
public InputActionProperty triggerAction;

public Animator myAnimator;

    // Update is called once per frame
    void Update()
    {
        float grabValue = grabAction.action.ReadValue<float>();
        myAnimator.SetFloat("Grab", grabValue);

        float triggerValue = triggerAction.action.ReadValue<float>();
        myAnimator.SetFloat("trigger", triggerValue);
    }
}
