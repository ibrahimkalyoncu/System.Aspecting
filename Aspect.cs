using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public U Call<U>(Expression<Func<T, U>> expressions)
        {
            return expressions.Compile().Invoke(_instance);
        }
    }
}
