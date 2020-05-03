using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeightDisplay : MonoBehaviour
{
    private Agent agentRef;
    // Start is called before the first frame update
    public Text title;
    public List<Text> weightList = new List<Text>(5);

    private GameObject selectedCube = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 50.0f)) {
                selectedCube = hit.transform.gameObject;
            }
        }

        DisplaySelectedCube(selectedCube);
    }

    private void DisplaySelectedCube(GameObject selectedCube)
    {
        if (selectedCube != null)
        {
            (int, int) position = selectedCube.GetComponent<FloorCube>().position;
            title.text = "X: " + position.Item1 + ", Y:" + position.Item2;
            agentRef = FindObjectOfType<Agent>();
            for (int i = 0; i < weightList.Capacity; ++i)
            {
                weightList[i].text = agentRef.StateActionPairQValue[(position, (Action)i)].ToString();
            }
        }
    }
}
