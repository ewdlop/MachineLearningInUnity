using System;
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
        IterationText.text = $"Iteration: {newText}";
    }

    public void UpdateStepText(string newText)
    {
        StepText.text = $"Step: {newText}";
    }
    public void UpdateLearningRateValue(float newValue)
    {
        LearningRateText.text = $"LearningRate: {newValue}";
    }

    public void UpdateDiscountingFactorValue(float newValue)
    {
        DiscountingFactorText.text = $"Discounting Factor: {newValue}";
    }

    public void UpdateStepTimeValue(float newValue)
    {
        StepTimeText.text = $"Step Time: {newValue}";
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
