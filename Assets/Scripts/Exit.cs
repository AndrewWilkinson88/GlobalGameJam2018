using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Exit : MonoBehaviour
{
    public List<Goal> goals;
    private bool isOpen = false;

	// Update is called once per frame
	void Update ()
    {
        if( !isOpen )
        {
            bool open = true;
            foreach( Goal goal in goals)
            {
                open = open && goal.CheckCharged();
            }

            if( open )
            {
                foreach (Goal goal in goals)
                {
                    goal.SetComplete();
                }

                OpenGate();
            }
        }
	}

    void OpenGate()
    {
        isOpen = true;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponentInChildren<ExitTrigger>(true).gameObject.SetActive(true);
    }
}
