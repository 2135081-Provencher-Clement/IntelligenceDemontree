using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// Deuxi�me cerveau de l'agent d�velopp�
/// 
/// Cherche � aller toucher une boule, mais esquive un ennemi en chemin
/// </summary>
public class GoatMouvementLongAgent : Agent
{
    /// <summary>
    /// Le mat�riel � appliquer au plancher lorsque l'agent r�ussi � toucher le but
    /// </summary>
    [SerializeField]
    private Material materielPlancherGagnant;

    /// <summary>
    /// Le mat�riel � appliquer au plancher lorsque l'agent touche un mur
    /// </summary>
    [SerializeField]
    private Material materielPlancherPerdant;

    /// <summary>
    /// Le Mesh renderer du plancher, pour pouvoir changer le mat�riel du plancher
    /// </summary>
    [SerializeField]
    private MeshRenderer plancher;

    /// <summary>
    /// La position de d�part de l'agent
    /// </summary>
    [SerializeField]
    private Transform positionDepart;

    /// <summary>
    /// La position de l'ennemi
    /// </summary>
    [SerializeField] 
    private Transform positionEnemy;

    /// <summary>
    /// La vitesse � laquelle l'Agent se d�place
    /// </summary>
    [SerializeField]
    private float vitesseDeplacement;

    /// <summary>
    /// La vitesse � laquelle l'agent se tourne
    /// </summary>
    [SerializeField]
    private float vitesseRotation;

    /// <summary>
    /// La position du but � atteindre
    /// </summary>
    [SerializeField]
    private Transform targetTransform;

    /// <summary>
    /// Envoie des observations au cerveau pour lui faire prendre une d�cision
    /// 
    /// Envoie les informations suivantes ->
    ///     La position de l'agent,
    ///     La position de l'ennemi,
    ///     La rotation de l'agent
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(positionEnemy.localPosition.x);
        sensor.AddObservation(positionEnemy.localPosition.z);
        sensor.AddObservation(transform.rotation.y);
    }

    /// <summary>
    /// Effectue les actions d�sir� par l'agent bas� sur les informations de d�sision re�ues du cerveau
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.DiscreteActions[0] == 1)
            PivoterGauche();

        if (actions.DiscreteActions[1] == 1)
            Avancer();

        if (actions.DiscreteActions[0] == 2)
            PivoterDroite();

        if (actions.DiscreteActions[1] == 2)
            Reculer();
    }

    /// <summary>
    /// Fait avancer l'agent
    /// </summary>
    private void Avancer()
    {
        transform.localPosition += vitesseDeplacement * transform.forward * Time.deltaTime;
    }

    /// <summary>
    /// Fait reculer l'agent
    /// </summary>
    private void Reculer()
    {
        transform.localPosition += vitesseDeplacement * -transform.forward * Time.deltaTime;
    }

    /// <summary>
    /// Fait pivoter l'agent � droite
    /// </summary>
    private void PivoterDroite()
    {
        transform.Rotate(Vector3.up * vitesseRotation * Time.deltaTime);
    }

    /// <summary>
    /// Fait pivoter l'agent � gauche
    /// </summary>
    private void PivoterGauche()
    {
        transform.Rotate(Vector3.down * vitesseRotation * Time.deltaTime);
    }

    /// <summary>
    /// M�thode qui s'occupe de faire bouger l'Agent s'il en mode heuristique (contr�l� par le joueur)
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actionSegment = actionsOut.DiscreteActions;
        actionSegment[0] = Input.GetKey(KeyCode.A) ? 1 : (Input.GetKey(KeyCode.D) ? 2 : 0);
        actionSegment[1] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
    }

    /// <summary>
    /// Ajoute des points � l'agent quand il touche le but
    /// Termine l'�pisode actuel
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Goal>() != null)
        {
            plancher.material = materielPlancherGagnant;
            SetReward(20.0f);
            EndEpisode();
        }
    }

    /// <summary>
    /// Retire des points � l'agent s'il touche un mur ou un ennemi
    /// Termine l'�pisode
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Mur>() != null || collision.gameObject.GetComponent<Enemy>())
        {
            plancher.material = materielPlancherPerdant;
            SetReward(-3f);
            EndEpisode();
        }
    }

    /// <summary>
    /// Lan�� lors du d�but d'un �pisode
    /// Replace l'agent � sa position de d�part
    /// </summary>
    public override void OnEpisodeBegin()
    {
        transform.localPosition = positionDepart.localPosition;
        transform.rotation = positionDepart.rotation;
    }
}
