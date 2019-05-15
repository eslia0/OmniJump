using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTest : MonoBehaviour {

    public Transform target;
    public Transform height;

    bool isJump = false;
    bool isChecked1 = false;
    bool isChecked2 = false;

    public float movePeriod = 1.0f;
    [SerializeField] float startTime;
    [SerializeField] float gravity;
    [SerializeField] float upSpeed;
    [SerializeField] float moveSpeed;

    GameObject checkObject1;
    GameObject checkObject2;

	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!isJump)
                Jump();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Reset();
        }

        if (isJump)
        {
            Move();
        }
	}

    void Reset()
    {
        isJump = false;
        isChecked1 = false;
        isChecked2 = false;
        transform.position = Vector3.zero;

        if (checkObject1)
            Destroy(checkObject1);

        if (checkObject2)
            Destroy(checkObject2);
    }

    void Jump()
    {
        // 각자의 높이
        float h1 = height.position.y - transform.position.y;
        float h2 = height.position.y - target.position.y;

        // x축 전체 이동 길이
        float dis = (target.position.x - transform.position.x);
        moveSpeed = dis;

        upSpeed = (1.0f + Mathf.Sqrt(h2 /h1)) * 2.0f * h1;
        gravity = Mathf.Pow(upSpeed, 2) / -2.0f / h1;
        // gravity = 2.0f * (h1 + h2) * (h1 + h2) / h2;
        // upSpeed = -gravity * h1 / (h1 + h2);
        isJump = true;
        startTime = 0.0f;
    } 

    void Move()
    {
        transform.Translate(new Vector3(moveSpeed, upSpeed) * Time.deltaTime / movePeriod);
        upSpeed += gravity * Time.deltaTime / movePeriod;

        startTime += Time.deltaTime;
        // Debug.Log(transform.position.y);

        if (startTime >= movePeriod / 2.0f && !isChecked1)
        {
            Debug.Log(upSpeed);
            isChecked1 = true;
            checkObject1 = Instantiate<GameObject>(Resources.Load<GameObject>("TestObject"), new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        }

        if (startTime >= movePeriod && !isChecked2)
        {
            Debug.Log(upSpeed);
            isChecked2 = true;
            checkObject2 = Instantiate<GameObject>(Resources.Load<GameObject>("TestObject"), new Vector3(transform.position.x, transform.position.y, 0 ), Quaternion.identity);
        }
    }

    public void ShowAds()
    {
        UnityAdsHelper.Instance.ShowRewardedAd();
    }
}
