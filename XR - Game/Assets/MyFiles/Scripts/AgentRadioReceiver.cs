using UnityEngine;

public class AgentRadioReceiver : MonoBehaviour
{
    private Agent agent;

    private void Awake()
    {
        agent = GetComponent<Agent>();
    }

    private void OnEnable()
    {
        ShadowMonsterAgent.OnShadowMonsterAlertTriggered += OnAlertReceived;
    }

    private void OnDisable()
    {
        ShadowMonsterAgent.OnShadowMonsterAlertTriggered -= OnAlertReceived;
    }

    private void OnAlertReceived(Vector3 location)
    {
        // Alleen reageren als hij niet al aan het chasen is
        if (agent.GetCurrentStateName() == "ChaseState") return;

        agent.ChangeState(new InvestigateState(agent, location));
    }
}