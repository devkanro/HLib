using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;

namespace HLib.Reflection
{
    /// <summary>
    /// 提供通用的注册动态类型的方法,可通过反射来调用
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    public class EventRegister<THandler>
    {
        public EventRegister(EventInfo eventInfo, Object targetObject)
        {
            EventInfo = eventInfo;
            TargetObject = targetObject;
        }
        
        public EventInfo EventInfo { get; set; }
        public Object TargetObject { get; set; }

        public void RegisteEvent(THandler handler)
        {
            WindowsRuntimeMarshal.AddEventHandler<THandler>(
                h => (EventRegistrationToken)EventInfo.AddMethod.Invoke(TargetObject, new object[] { h }), 
                t => EventInfo.RemoveMethod.Invoke(TargetObject, new object[] { t }), 
                handler);
        }
    }

    /// <summary>
    /// 提供通用的注册动态类型的方法,使用反射封装了 <see cref="EventRegister{THandler}"/> ,更易于使用
    /// </summary>
    public class EventRegister
    {
        public EventRegister(EventInfo eventInfo, Object targetObject)
        {
            RegisterClass = new Class(typeof(EventRegister<>));
            RegisterClass.MakeGenericType(eventInfo.EventHandlerType);
            RegisterClass.TargetObject = RegisterClass.CreateInstance(eventInfo, targetObject);
        }

        public Class RegisterClass { get; private set; }

        public void RegisteEvent(Delegate handler)
        {
            RegisterClass.InvokeMethod("RegisteEvent", handler);
        }
    }
}
