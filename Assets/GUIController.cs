using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[Serializable]
public class GUIController : MonoBehaviour
{
    [SerializeField]
    private Text _iterationText;
    [SerializeField]
    private Text _stepText;
    public Text IterationText { get => _iterationText; set => _stepText = value; }
    public Text StepText { get => _stepText; set => _stepText = value; }

    public void UpdateInterationText(string newText)
    {
        IterationText.text = string.Format("Iteration: {0}", newText);
    }

    public void UpdateStepText(string newText)
    {
        StepText.text = string.Format("Step: {0}", newText);
    }

    public void ResetScene()
    {       
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
