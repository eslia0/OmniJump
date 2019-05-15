using System.Collections;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    Direction direction;
    bool isChecked;
    PlayerController m_player;

    [SerializeField] GameObject returnPoint;

    void Awake()
    {
        m_player = StageManager.Instance.player;
    }

    // dir : 체크되는 포인트
    // checkType : 플레이어가 정지된 뒤에 체크하는지 / 플레이어가 이동한 뒤 특정 위치로 돌아오는지
    public IEnumerator SetPoint(Direction dir, bool checkType)
    {
        isChecked = false;
        direction = dir;

        if (checkType)
        {
            while (direction != (Direction)m_player.faceDirection)
            {
                yield return null;
            }
        }
        else
        {
            while (direction != (Direction)m_player.faceDirection)
            {
                if (Vector3.Distance(m_player.transform.position, transform.position) < 0.32f)
                {
                    m_player.transform.position = returnPoint.transform.position;
                }

                yield return null;
            }
        }

        isChecked = true;
    }

    public IEnumerator SetPoint(InteractionManager interactObject, bool checkType)
    {
        isChecked = false;

        if (checkType)
        {
            while (interactObject.Input != 0)
            {
                yield return null;
            }
        }
        else
        {
            while (interactObject.Input != 0)
            {
                if (Vector3.Distance(m_player.transform.position, transform.position) < 0.32f)
                {
                    m_player.transform.position = returnPoint.transform.position;
                }

                yield return null;
            }
        }

        isChecked = true;
    }
}
