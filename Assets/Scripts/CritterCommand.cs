public abstract class CritterCommand
{
    public abstract void Execute();
    public abstract bool IsFinished {get;}
    public abstract void OnArrive();
}