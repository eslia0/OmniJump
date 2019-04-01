using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionRandomizer : MonoBehaviour
{
    private List<Direction> directions = new List<Direction>();
    private int currentMaxArrayPosition = 3;

    public DirectionRandomizer(){
        for(int i = 0; i < 4; i++)
        {
            directions.Add((Direction)i);
        }
    }

    public Direction RandomizeDirection()
    {
        if (currentMaxArrayPosition < 0)
        {
            currentMaxArrayPosition = 3;
        }

        int arrayPosition = Random.RandomRange(0,currentMaxArrayPosition);
        Direction selectedDirection = directions[arrayPosition];

        directions.RemoveAt(arrayPosition);
        directions.Add(selectedDirection);
        // Debug.Log("selected(" + selectedDirection + "), Max : " + currentMaxArrayPosition + ", " + directions[0] + ", " + directions[1] + ", " + directions[2] + ", " + directions[3]);

        currentMaxArrayPosition--;

        return selectedDirection;
    }
}
