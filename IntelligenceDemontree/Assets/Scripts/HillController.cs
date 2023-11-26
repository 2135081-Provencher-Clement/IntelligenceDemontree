using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contr�leur pour un ar�ne de "King of the hill" Dans lequel �volue 4 agents
/// 
/// D�place un point � atteindre apr�s une certaine dur�e de temps
/// </summary>
public class HillController : MonoBehaviour
{
    /// <summary>
    /// Liste des agents pr�sents
    /// </summary>
    [SerializeField]
    private KingGoatAgent[] goats;

    /// <summary>
    /// Le point � atteindre
    /// </summary>
    [SerializeField]
    private GameObject target;

    /// <summary>
    /// Positions possibles pour le point � atteindre
    /// </summary>
    [SerializeField]
    private Transform[] positionsPossibles;

    /// <summary>
    /// Temps �coul� depuis dernier d�placement
    /// </summary>
    private float tempsDepuisDebutVague = 0.0f;

    /// <summary>
    /// Temps � attendre avant de d�placer de nouveau
    /// </summary>
    private float nbSecondesParRound = 15.0f;

    /// <summary>
    /// Termine le "round" si �a fait assez longtemps qu'il dure
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
    /// Indique � chaque agent que le "round" est termin� et d�place ensuite le point
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
    /// D�place le point � atteindre
    /// </summary>
    private void DeplacerTarget()
    {
        int indexTarget = Random.Range(0, 4);
        target.transform.position = positionsPossibles[indexTarget].position;
    }
}
