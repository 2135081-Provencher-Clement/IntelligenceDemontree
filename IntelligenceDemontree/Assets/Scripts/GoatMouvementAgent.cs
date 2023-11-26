using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// Premier cerveau de l'agent développé
/// 
/// Cherche seulement à aller toucher une boule
/// </summary>
public class GoatMouvementAgent : Agent
{
    /// <summary>
    /// Le matériel à appliquer au plancher lorsque l'agent réussi à toucher le but
    /// </summary>
    [SerializeField]
    private Material materielPlancherGagnant;

    /// <summary>
    /// Le matériel à appliquer au plancher lorsque l'agent touche un mur
    /// </summary>
    [SerializeField]
    private Material materielPlancherPerdant;

    /// <summary>
    /// Le Mesh renderer du plancher, pour pouvoir changer le matériel du plancher
    /// </summary>
    [SerializeField]
    private MeshRenderer plancher;

    /// <summary>
    /// La position de départ de l'agent
    /// </summary>
    [SerializeField]
    private Transform positionDepart;

    /// <summary>
    /// La vitesse à laquelle l'Agent se déplace
    /// </summary>
    [SerializeField]
    private float vitesseDeplacement;

    /// <summary>
    /// La vitesse à laquelle l'agent se tourne
    /// </summary>
    [SerializeField]
    private float vitesseRotation;

    /// <summary>
    /// La position du but à atteindre
    /// </summary>
    [SerializeField]
    private Transform targetTransform;

    /// <summary>
    /// Envoie des observations au cerveau pour lui faire prendre une décision
    /// 
    /// Envoie les informations suivantes ->
    ///     La position de l'agent,
    ///     La position du but,
    ///     La rotation de l'agent
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(targetTransform.localPosition.x);
        sensor.AddObservation(targetTransform.localPosition.z);
        sensor.AddObservation(transform.rotation.y);
    }

    /// <summary>
    /// Effectue les actions désiré par l'agent basé sur les informations de désision reçues du cerveau
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
    /// Fait pivoter l'agent à droite
    /// </summary>
    private void PivoterDroite()
    {
        transform.Rotate(Vector3.up * vitesseRotation * Time.deltaTime);
    }

    /// <summary>
    /// Fait pivoter l'agent à gauche
    /// </summary>
    private void PivoterGauche()
    {
        transform.Rotate(Vector3.down * vitesseRotation * Time.deltaTime);
    }

    /// <summary>
    /// Méthode qui s'occupe de faire bouger l'Agent s'il en mode heuristique (contrôlé par le joueur)
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actionSegment = actionsOut.DiscreteActions;
        actionSegment[0] = Input.GetKey(KeyCode.A) ? 1 : (Input.GetKey(KeyCode.D) ? 2 : 0);
        actionSegment[1] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
    }

    /// <summary>
    /// Ajoute des points à l'agent quand il touche le but
    /// Termine l'épisode actuel
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Goal>() != null)
        {
            plancher.material = materielPlancherGagnant;
            SetReward(5f);
            EndEpisode();
        }
    }

    /// <summary>
    /// Retire des points à l'agent s'il touche un mur
    /// Termine l'épisode
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Mur>() != null) 
        {
            plancher.material = materielPlancherPerdant;
            SetReward(-3f);
            EndEpisode();
        }
    }

    /// <summary>
    /// Lançé lors du début d'un épisode
    /// Replace l'agent à sa position de départ
    /// Bouge le but
    /// </summary>
    public override void OnEpisodeBegin()
    {
        transform.localPosition = positionDepart.localPosition;
        transform.rotation = positionDepart.rotation;
        BougerGoal();
    }

    /// <summary>
    /// Déplace l'emplacement du but
    /// </summary>
    private void BougerGoal()
    {
        targetTransform.localPosition = new Vector3(
            Random.value > 0.5f ? Random.Range(3.0f, 15.0f) : Random.Range(-3.0f, -15.0f),
            0,
            Random.value > 0.5f ? Random.Range(5.0f, 15.0f) : Random.Range(-5.0f, -15.0f)
            );
    }
}
