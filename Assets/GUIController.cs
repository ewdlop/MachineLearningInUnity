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
    [SerializeField]
    private Text _learningRateText;
    [SerializeField]
    private Text _discountingFactorText;
    [SerializeField]
    private Text _stepTimeText;
    public Text IterationText { get => _iterationText; set => _iterationText = value; }
    public Text StepText { get => _stepText; set => _stepText = value; }
    public Text LearningRateText { get => _learningRateText; set => _learningRateText = value; }
    public Text DiscountingFactorText { get => _discountingFactorText; set => _discountingFactorText = value; }
    public Text StepTimeText { get => _stepTimeText; set => _stepTimeText = value; }
    public GameObject weightDisplayPanel;

    public void UpdateInterationText(string newText)
    {
        IterationText.text = string.Format("Iteration: {0}", newText);
    }

    public void UpdateStepText(string newText)
    {
        StepText.text = string.Format("Step: {0}", newText);
    }
    public void UpdateLearningRateValue(float newValue)
    {
        LearningRateText.text = string.Format("LearningRate: {0}", newValue);
    }

    public void UpdateDiscountingFactorValue(float newValue)
    {
        DiscountingFactorText.text = string.Format("Discounting Factor: {0}", newValue);
    }

    public void UpdateStepTimeValue(float newValue)
    {
        StepTimeText.text = string.Format("Step Time: {0}", newValue);
    }
    public void ResetScene()
    {       
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ShowWeightDisplay()
    {
        weightDisplayPanel.SetActive(true);
    }

    public void HideWeightDisplay()
    {
        weightDisplayPanel.SetActive(false);
    }
}
