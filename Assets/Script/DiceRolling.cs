using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRolling : MonoBehaviour
{
    private int tryGetResultAmount = 0;
    private int rerollAmount = 0;
    void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddForce(Physics.gravity * GetComponent<Rigidbody>().mass);
    }

    public void RollTheDice()
    {
        tryGetResultAmount = 0;
        transform.rotation = Random.rotation;
        GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1.0f,1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 1000, ForceMode.Impulse);
        GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -2), ForceMode.VelocityChange);
        Invoke("ReturnDiceResult", 1.5f);
    }

    public void ReturnDiceResult()
    {
        int diceResult = 0;

        if(Vector3.Dot(transform.forward, Vector3.up) > 0.8f)
            diceResult = 1;
        else if (Vector3.Dot(-transform.forward, Vector3.up) > 0.8f)
            diceResult = 6;
        else if (Vector3.Dot(transform.up, Vector3.up) > 0.8f)
            diceResult = 2;
        else if (Vector3.Dot(-transform.up, Vector3.up) > 0.8f)
            diceResult = 5;
        else if (Vector3.Dot(transform.right, Vector3.up) > 0.8f)
            diceResult = 4;
        else if (Vector3.Dot(-transform.right, Vector3.up) > 0.8f)
            diceResult = 3;

        if(diceResult == 0)
        {
            if(tryGetResultAmount < 1)
            {
                Invoke("ReturnDiceResult", 1.0f);
                tryGetResultAmount++;
                return;
            }
            else if(rerollAmount < 2)
            {
                Vector3 curPos = transform.position;
                curPos.y = 4;
                transform.position = curPos;
                rerollAmount++;
                RollTheDice();
                return;
            }
        }

        GameManager.Instance.OnDiceResult(gameObject, diceResult);
    }
}
