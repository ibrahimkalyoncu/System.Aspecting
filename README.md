# System.Aspecting
An opensource project targets to bring AOP into c#. Its still being developed. Currently method executions can be handeled.

## Creating an aspect attribute
```cs
public class SampleMethodAspectAttribute : AspectMethodAttribute
{
    public override void OnExecuting(AspectCall call)
    {
        if(call.Arguments.Length == 0)
          call.Canceled = true;
    }

    public override void OnExecuted(AspectCall call)
    {
        if ((int)call.Result > 10)
            call.Result = -1;
    }

    public override void OnException(AspectCall call, Exception exception)
    {
        call.Result = -2;
        call.Exception = exception;
    }
}
```

## Marking methods with attributes
```cs
public class MyClass
{
    [SampleMethodAspect]
    public int Add(params int[] nums)
    {
        return nums.Sum();
    }
}
```

## Invoking methods
```cs
//Normal usage
MyClass mC = new MyClass();
int result = mC.Add(5,10,15);

//Aspect usage
Aspect<MyClass> aspectMC = new Aspect<MyClass>();
result = aspectMC.Invoke(mc => mc.Add(5,10,15));
```
