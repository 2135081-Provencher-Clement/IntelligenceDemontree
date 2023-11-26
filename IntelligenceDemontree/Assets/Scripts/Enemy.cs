using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script pour d�placer l'ennemi
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// La direction dans laquelle l'ennemi se d�place (soit 1 ou -1)
    /// </summary>
    private int direction = 1;

    /// <summary>
    /// La vitesse � laquelle l'ennemi se d�place
    /// </summary>
    [SerializeField]
    private float vitesseDeplacement = 3.0f;

    /// <summary>
    /// La distance � laquelle l'ennemi doit changer de direction de d�placement
    /// </summary>
    [SerializeField]
    private int maxDistance;

    /// <summary>
    /// V�rifie si l'ennemi s'ent rendu � sa distance maximal, si oui, change de direction
    /// Sinon, d�place l'ennemi
    /// </summary>
    private void FixedUpdate()
    {
        float nouvellePos = transform.localPosition.x + maxDistance/vitesseDeplacement * Time.deltaTime * direction;

        if (nouvellePos >= maxDistance && direction == 1)
        {
            direction = -1;
        }
        else if(nouvellePos <= -maxDistance && direction == -1)
        {
            direction = 1;
        }

        transform.localPosition = new Vector3(nouvellePos, 1, 0);
    }
}
