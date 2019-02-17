using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    private int currentTile;
    [SerializeField]
    private float speed = 1.0f;

    public Text currentTileText;

    // Start is called before the first frame update
    void Start()
    {
        currentTile = 0;
    }

    // Update is called once per frame
    void Update()
    {
        currentTileText.text = currentTile + "";
    }

    public void updateTile(int steps)
    {
        StartCoroutine(TileTransitionRoutine(steps));
    }

    IEnumerator TileTransitionRoutine(int steps)
    {
        for (int i = 0; i < steps; i++)
        {
            Vector3 targetPosition = GameManager.Instance.GetTilePosition(currentTile + i).position;

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, (speed)*Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            transform.position = targetPosition;
        }
        currentTile += steps;
        GameManager.Instance.ChangeDiceRollStatus(false);
    }

}
