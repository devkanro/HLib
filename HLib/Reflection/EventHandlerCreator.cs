using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLib.Reflection
{
    /// <summary>
    /// 提供创建拥有两个参数无返回值指定的类型的签名的方法,可通过反射来调用
    /// </summary>
    /// <typeparam name="T1">参数1的类型</typeparam>
    /// <typeparam name="T2">参数2的类型</typeparam>
    public class EventHandlerCreator<T1, T2>
    {
        /// <summary>
        /// 生成的方法,调用此方法会执行 <see cref="EventHandlerCreator{T1, T2}.Handler"/> 委托
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        public void Triggle(T1 arg1,T2 arg2)
        {
            Handler?.Invoke(arg1, arg2);
        }

        /// <summary>
        /// 需要执行的委托
        /// </summary>
        public Action<object, object> Handler { get; set; }
    }
}