using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformSpeedControl : MonoBehaviour
{
    public MovingPlatform[] scripts;
    public List<MovingSwitch> checkBoxes;


    private void Start()
    {
        if(scripts.Length < 0)
        {
            enabled = false;
            return;
        }
        checkBoxes.AddRange(transform.GetComponentsInChildren<MovingSwitch>());
    }

    void Update()
    {
        for (int i = 0; i < checkBoxes.Count; i++)
        {
            if (checkBoxes[i] != null)
            {
                Collider2D check = Physics2D.OverlapBox(checkBoxes[i].transform.position, new Vector2(0.32f, 0.32f), 0f, EndlessManager.Instance.playerLayer);

                if (check 
                    && EndlessManager.Instance.player.interactionDirection == checkBoxes[i].direction 
                    && EndlessManager.Instance.player.onClick)
                {
                    Destroy(checkBoxes[i].gameObject);
                    checkBoxes.Remove(checkBoxes[i]);
                    for(int j = 0; j < scripts.Length; j++)
                    {
                        scripts[j].percentBetweenWaypoints -= 0.16f;
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int i = 0; i < checkBoxes.Count; i++)
        {
            if (checkBoxes[i] != null)
            {
                Gizmos.DrawWireCube(checkBoxes[i].transform.position, new Vector2(0.32f, 0.32f));
            }
        }
    }
}
