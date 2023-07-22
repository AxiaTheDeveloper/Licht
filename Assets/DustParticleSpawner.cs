using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustParticleSpawner : MonoBehaviour
{
    Vector3 posStay;
    SpriteRenderer dustPSprite;
    Animator dustTrailAnimator;

    // Update is called once per frame
    void Update()
    {
        transform.position = posStay;
        dustTrailAnimator = GetComponent<Animator>();
        dustPSprite = GetComponent<SpriteRenderer>();
    }

    public void SummonDustTrail(Vector3 playerPosition, float direction)
    {

        if (direction < 0)
        {
            posStay = playerPosition + new Vector3(0.5f, 0f, 0f);
            dustPSprite.flipX = true;
        }
        else
        {
            posStay = playerPosition - new Vector3(0.5f, 0f, 0f);
            dustPSprite.flipX = false;
        }
        
        dustTrailAnimator.SetTrigger("PlayAnimation");
    }
}
