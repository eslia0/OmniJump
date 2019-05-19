using System.Collections;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] Direction direction;
    bool isChecked;
    PlayerController m_player;
    [SerializeField] InteractionManager interactObject;
    [SerializeField] bool checkType;

    Transform returnPoint;

    void Awake()
    {
        m_player = Creater.Instance.player;
        returnPoint = transform.GetChild(0);

        if(interactObject)
        {
            StartCoroutine(SetPoint(interactObject, checkType));
        }
        else
        {
            StartCoroutine(SetPoint(direction, checkType));
        }
    }

    // dir : 체크되는 포인트
    // checkType : 플레이어가 정지된 뒤에 체크하는지 / 플레이어가 이동한 뒤 특정 위치로 돌아오는지
    public IEnumerator SetPoint(Direction dir, bool checkType)
    {
        isChecked = false;
        direction = dir;
        
        if (checkType)
        {
            while (Vector3.Distance(transform.position, m_player.transform.position) > 0.32f)
            {
                yield return null;
            }

            m_player.enabled = false;

            while (direction != (Direction)m_player.faceDirection)
            {
                yield return null;
            }
        }
        else
        {
            while (direction != (Direction)m_player.faceDirection)
            {
                if (Vector3.Distance(m_player.transform.position, transform.position) < 0.16f)
                {
                    yield return new WaitForSeconds(0.5f);
                    m_player.transform.position = returnPoint.position;
                }
            }
        }
        
        m_player.enabled = true;
        isChecked = true;
    }

    // interactObject : 오브젝트의 사용됨을 판별
    // checkType : 플레이어가 정지된 뒤에 체크하는지 / 플레이어가 이동한 뒤 특정 위치로 돌아오는지
    public IEnumerator SetPoint(InteractionManager interactObject, bool checkType)
    {
        isChecked = false;

        if (checkType)
        {
            while (Vector3.Distance(transform.position, m_player.transform.position) > 0.16f)
            {
                yield return null;
            }

            m_player.enabled = false;

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
                    yield return new WaitForSeconds(0.5f);
                    m_player.transform.position = returnPoint.position;
                }
            }
        }

        m_player.enabled = true;
        isChecked = true;
    }
}
