using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public Text iterationText;
    public Text stepText;

    public void UpdateIterationText(int iteration)
    {
        iterationText.text = string.Format("Iteration: {0}",iteration);
    }

    public void UpdateStepText(int step)
    {
        stepText.text = string.Format("Step: {0}", step);
    }

}
