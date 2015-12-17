using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System.Aspecting
{
    public class Aspect<T> where T : class
    {
        private T _instance;

        public Aspect(params object[] args)
        {
            Type type = typeof(T);
            _instance = Activator.CreateInstance(type, args) as T;
        }

        public Aspect(Func<T> creator)
        {
            _instance = creator.Invoke();
        }

        public U Invoke<U>(Expression<Func<T, U>> expression)
        {
            AspectCall aspectCall = PreInvoke(expression);

            IEnumerable<AspectMethodAttribute> methodCallAttributes = aspectCall.MethodInfo.GetCustomAttributes(typeof(AspectMethodAttribute)) as IEnumerable<AspectMethodAttribute>;

            foreach (var attr in methodCallAttributes)
            {
                attr.OnExecuting(aspectCall);

                if (aspectCall.Exception != null)
                    throw aspectCall.Exception;

                if (aspectCall.Canceled)
                    return default(U);

                if (aspectCall.Result != null)
                    break;
            }

            if (aspectCall.Result == null)
                try
                {
                    aspectCall.Result = expression.Compile().Invoke(_instance);
                }
                catch (Exception exception)
                {
                    foreach (var attr in methodCallAttributes)
                    {
                        attr.OnException(aspectCall, exception);

                        if (aspectCall.Exception != null)
                            throw aspectCall.Exception;

                        if (aspectCall.Canceled)
                            return default(U);
                    }
                }

            foreach (var attr in methodCallAttributes)
                attr.OnExecuted(aspectCall);

            return (U)aspectCall.Result;
        }

        public void Invoke(Expression<Action<T>> expression)
        {
            AspectCall aspectCall = PreInvoke(expression);

            IEnumerable<AspectMethodAttribute> methodCallAttributes = aspectCall.MethodInfo.GetCustomAttributes(typeof(AspectMethodAttribute)) as IEnumerable<AspectMethodAttribute>;

            foreach (var attr in methodCallAttributes)
            {
                attr.OnExecuting(aspectCall);

                if (aspectCall.Canceled)
                    return;
            }

            try
            {
                expression.Compile().Invoke(_instance);
            }
            catch (Exception exception)
            {
                foreach (var attr in methodCallAttributes)
                {
                    attr.OnException(aspectCall, exception);

                    if (aspectCall.Exception != null)
                        throw aspectCall.Exception;

                    if (aspectCall.Canceled)
                        return;
                }
            }

            foreach (var attr in methodCallAttributes)
                attr.OnExecuted(aspectCall);
        }

        private AspectCall PreInvoke(LambdaExpression expression)
        {
            MethodCallExpression methodCall = expression.Body as MethodCallExpression;
            AspectCall aspectCall = new AspectCall();

            if (methodCall == null)
                throw new InvalidOperationException("Expression has to be a method call.");

            string methodName = methodCall.Method.Name;
            Type type = typeof(T);
            aspectCall.MethodInfo = type.GetMethod(methodName);
            aspectCall.Arguments = new object[methodCall.Arguments.Count];

            foreach (Expression argument in methodCall.Arguments)
            {
                LambdaExpression lambda = Expression.Lambda(argument, expression.Parameters);
                object value = lambda.Compile().DynamicInvoke(new object[1]);
                aspectCall.Arguments[methodCall.Arguments.IndexOf(argument)] = value;
            }

            return aspectCall;
        }
    }
}
