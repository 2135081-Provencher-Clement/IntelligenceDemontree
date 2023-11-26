using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// Troisième et dernier cerveau de l'agent développé
/// 
/// Tente de se déplacer vers un point
/// 
/// Particularité de cet agent -> il peut sauter
/// 
/// L'agent n'est pas très bon, particulièremement pour aller au point en haut à gauche
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class KingGoatAgent : Agent
{
    /// <summary>
    /// Le composant rigidbody de l'agent
    /// </summary>
    private Rigidbody rigidbody;

    /// <summary>
    /// Assigne le rigidbody lors de la création et assigne aussi la position de départ
    /// </summary>
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        positionDepart = rigidbody.transform.localPosition;
    }

    /// <summary>
    /// La position de départ de l'agent
    /// </summary>
    private Vector3 positionDepart;

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
    /// La position du point à atteindre
    /// </summary>
    [SerializeField]
    private Transform targetTransform;

    /// <summary>
    /// Envoie des observations au cerveau pour lui faire prendre une décision
    /// 
    /// Envoie les informations suivantes ->
    ///     La position de l'agent (y inclu),
    ///     La position du point à atteindre (y inclu),
    ///     La rotation de l'agent
    /// </summary>
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(transform.localPosition.z);
        sensor.AddObservation(targetTransform.localPosition.x);
        sensor.AddObservation(targetTransform.localPosition.y);
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

        if (actions.DiscreteActions[2] == 1)
            Sauter();
    }

    /// <summary>
    /// Fait avancer l'agent
    /// </summary>
    private void Avancer()
    {
        rigidbody.transform.localPosition += vitesseDeplacement * transform.forward * Time.deltaTime;
    }

    /// <summary>
    /// Fait reculer l'agent
    /// </summary>
    private void Reculer()
    {
        rigidbody.transform.localPosition += vitesseDeplacement * -transform.forward * Time.deltaTime;
    }

    /// <summary>
    /// Fait pivoter l'agent à droite
    /// </summary>
    private void PivoterDroite()
    {
        rigidbody.transform.Rotate(Vector3.up * vitesseRotation * Time.deltaTime);
    }

    /// <summary>
    /// Fait pivoter l'agent à gauche
    /// </summary>
    private void PivoterGauche()
    {
        rigidbody.transform.Rotate(Vector3.down * vitesseRotation * Time.deltaTime);
    }

    /// <summary>
    /// Indique si l'agent est au sol
    /// </summary>
    /// <returns>Vrai si l'agent touche au sol</returns>
    bool IsGrounded()
    {
        return rigidbody.velocity.y == 0;
    }

    /// <summary>
    /// Fait sauter l'agent
    /// </summary>
    private void Sauter()
    {
        if (IsGrounded())
        {
            rigidbody.AddForce(new Vector3(0, 100, 0), ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Méthode qui s'occupe de faire bouger l'Agent s'il en mode heuristique (contrôlé par le joueur)
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actionSegment = actionsOut.DiscreteActions;
        actionSegment[0] = Input.GetKey(KeyCode.A) ? 1 : (Input.GetKey(KeyCode.D) ? 2 : 0);
        actionSegment[1] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
        actionSegment[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    /// <summary>
    /// Lors d'une collision avec un mur, pénalise l'agent et replace l'Agent à sa position de départ
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Mur>() != null)
        {

            EndEpisode();
            SetReward(-3.0f);
            transform.localPosition = positionDepart;
            transform.rotation = Quaternion.identity;
        }
    }

    /// <summary>
    /// Assigne une récompense à l'agent selon sa distance au point
    /// plus il est près du point, plus il recoit un bonus considérable
    /// 
    /// puis, replace l'Agent à sa position de départ
    /// </summary>
    public void SuccessfullRound()
    {
        if (Vector3.Distance(rigidbody.transform.localPosition, targetTransform.localPosition) <= 10)
        {
            SetReward(70.0f);
        }
        else if (Vector3.Distance(rigidbody.transform.localPosition, targetTransform.localPosition) <= 20)
        {
            SetReward(25.0f);
        }
        else if (Vector3.Distance(rigidbody.transform.localPosition, targetTransform.localPosition) <= 30)
        {
            SetReward(15.0f);
        }
        else if (Vector3.Distance(rigidbody.transform.localPosition, targetTransform.localPosition) <= 50)
        {
            SetReward(5.0f);
        }
        else if (Vector3.Distance(rigidbody.transform.localPosition, targetTransform.localPosition) <= 65)
        {
            SetReward(0.01f);
        }

        transform.localPosition = positionDepart;
        transform.rotation = Quaternion.identity;

        EndEpisode();
    }
}
