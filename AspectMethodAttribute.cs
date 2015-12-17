namespace System.Aspecting
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public abstract class AspectMethodAttribute : Attribute
    {
        public abstract void OnExecuting(AspectCall call);
        public abstract void OnExecuted(AspectCall call);
    }
}
