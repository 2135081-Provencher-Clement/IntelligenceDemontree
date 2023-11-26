using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script pour déplacer l'ennemi
/// </summary>
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// La direction dans laquelle l'ennemi se déplace (soit 1 ou -1)
    /// </summary>
    private int direction = 1;

    /// <summary>
    /// La vitesse à laquelle l'ennemi se déplace
    /// </summary>
    [SerializeField]
    private float vitesseDeplacement = 3.0f;

    /// <summary>
    /// La distance à laquelle l'ennemi doit changer de direction de déplacement
    /// </summary>
    [SerializeField]
    private int maxDistance;

    /// <summary>
    /// Vérifie si l'ennemi s'ent rendu à sa distance maximal, si oui, change de direction
    /// Sinon, déplace l'ennemi
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
