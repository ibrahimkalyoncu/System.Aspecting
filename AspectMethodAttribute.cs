namespace System.Aspecting
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class AspectMethodAttribute : Attribute
    {
        public virtual void OnExecuting(AspectCall call) { }
        public virtual void OnExecuted(AspectCall call) { }
        public virtual void OnException(AspectCall call, Exception exception) { }
    }
}
