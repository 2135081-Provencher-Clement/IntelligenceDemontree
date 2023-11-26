using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

/// <summary>
/// Troisi�me et dernier cerveau de l'agent d�velopp�
/// 
/// Tente de se d�placer vers un point
/// 
/// Particularit� de cet agent -> il peut sauter
/// 
/// L'agent n'est pas tr�s bon, particuli�remement pour aller au point en haut � gauche
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class KingGoatAgent : Agent
{
    /// <summary>
    /// Le composant rigidbody de l'agent
    /// </summary>
    private Rigidbody rigidbody;

    /// <summary>
    /// Assigne le rigidbody lors de la cr�ation et assigne aussi la position de d�part
    /// </summary>
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        positionDepart = rigidbody.transform.localPosition;
    }

    /// <summary>
    /// La position de d�part de l'agent
    /// </summary>
    private Vector3 positionDepart;

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
    /// La position du point � atteindre
    /// </summary>
    [SerializeField]
    private Transform targetTransform;

    /// <summary>
    /// Envoie des observations au cerveau pour lui faire prendre une d�cision
    /// 
    /// Envoie les informations suivantes ->
    ///     La position de l'agent (y inclu),
    ///     La position du point � atteindre (y inclu),
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
    /// Fait pivoter l'agent � droite
    /// </summary>
    private void PivoterDroite()
    {
        rigidbody.transform.Rotate(Vector3.up * vitesseRotation * Time.deltaTime);
    }

    /// <summary>
    /// Fait pivoter l'agent � gauche
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
    /// M�thode qui s'occupe de faire bouger l'Agent s'il en mode heuristique (contr�l� par le joueur)
    /// </summary>
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> actionSegment = actionsOut.DiscreteActions;
        actionSegment[0] = Input.GetKey(KeyCode.A) ? 1 : (Input.GetKey(KeyCode.D) ? 2 : 0);
        actionSegment[1] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? 2 : 0);
        actionSegment[2] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    /// <summary>
    /// Lors d'une collision avec un mur, p�nalise l'agent et replace l'Agent � sa position de d�part
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
    /// Assigne une r�compense � l'agent selon sa distance au point
    /// plus il est pr�s du point, plus il recoit un bonus consid�rable
    /// 
    /// puis, replace l'Agent � sa position de d�part
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
