public abstract class State
{
    protected Agent agent;

    public State(Agent agent)
    {
        this.agent = agent;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }
}