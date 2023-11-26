using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôleur pour un arène de "King of the hill" Dans lequel évolue 4 agents
/// 
/// Déplace un point à atteindre après une certaine durée de temps
/// </summary>
public class HillController : MonoBehaviour
{
    /// <summary>
    /// Liste des agents présents
    /// </summary>
    [SerializeField]
    private KingGoatAgent[] goats;

    /// <summary>
    /// Le point à atteindre
    /// </summary>
    [SerializeField]
    private GameObject target;

    /// <summary>
    /// Positions possibles pour le point à atteindre
    /// </summary>
    [SerializeField]
    private Transform[] positionsPossibles;

    /// <summary>
    /// Temps écoulé depuis dernier déplacement
    /// </summary>
    private float tempsDepuisDebutVague = 0.0f;

    /// <summary>
    /// Temps à attendre avant de déplacer de nouveau
    /// </summary>
    private float nbSecondesParRound = 15.0f;

    /// <summary>
    /// Termine le "round" si ça fait assez longtemps qu'il dure
    /// </summary>
    private void Update()
    {
        tempsDepuisDebutVague += Time.deltaTime;

        if(tempsDepuisDebutVague >= nbSecondesParRound )
        {
            FinRound();
        }
    }

    /// <summary>
    /// Indique à chaque agent que le "round" est terminé et déplace ensuite le point
    /// </summary>
    private void FinRound()
    {
        tempsDepuisDebutVague = 0.0f;

        foreach(KingGoatAgent goat in goats)
        {
            goat.SuccessfullRound();
        }

        DeplacerTarget();
    }

    /// <summary>
    /// Déplace le point à atteindre
    /// </summary>
    private void DeplacerTarget()
    {
        int indexTarget = Random.Range(0, 4);
        target.transform.position = positionsPossibles[indexTarget].position;
    }
}
