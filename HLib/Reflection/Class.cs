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
    /// 表示一个类,提供相关的反射访问
    /// </summary>
    public class Class
    {
        /// <summary>
        /// 通过一个实例化对象获取类信息
        /// </summary>
        /// <param name="obj"></param>
        public Class(Object obj)
        {
            TargetObject = obj;
            ClassType = TargetObject.GetType();
        }

        /// <summary>
        /// 通过一个类型获取类信息,稍后可使用 <see cref="Class.CreateInstance"/> 方法创建实例,设置 <see cref="Class.TargetObject"/>
        /// </summary>
        /// <param name="type"></param>
        public Class(Type type)
        {
            ClassType = type;
        }

        /// <summary>
        /// 获取或设置获取属性,字段,方法时的实例对象
        /// </summary>
        public Object TargetObject { get; set; }

        /// <summary>
        /// 获取该对象的类型信息
        /// </summary>
        public Type ClassType { get; private set; }

        /// <summary>
        /// 将泛型定义信息写入当前类中
        /// </summary>
        /// <param name="parameters"></param>
        public void MakeGenericType(params Type[] parameters)
        {
            ClassType = ClassType.MakeGenericType(parameters);
        }

        /// <summary>
        /// 根据属性名获取属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetProperty(String name)
        {
            return ClassType.GetRuntimeProperty(name).GetValue(TargetObject);
        }

        /// <summary>
        /// 根据属性名获取属性,并转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetProperty<T>(String name)
        {
            return (T)ClassType.GetRuntimeProperty(name).GetValue(TargetObject);
        }

        /// <summary>
        /// 根据属性名设置属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(String name,object value)
        {
            ClassType.GetRuntimeProperty(name).SetValue(TargetObject, value);
        }

        /// <summary>
        /// 根据属性名与索引,获取属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public object GetProperty(String name, object[] index)
        {
            return ClassType.GetRuntimeProperty(name).GetValue(TargetObject, index);
        }

        /// <summary>
        /// 根据属性名与索引,获取属性值,并转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public T GetProperty<T>(String name, object[] index)
        {
            return (T)ClassType.GetRuntimeProperty(name).GetValue(TargetObject, index);
        }

        /// <summary>
        /// 根据属性名与索引,设置属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public void SetProperty(String name, object value, object[] index)
        {
            ClassType.GetRuntimeProperty(name).SetValue(TargetObject, value, index);
        }

        /// <summary>
        /// 根据方法名执行一个方法,并提供相应的参数,对于重载方法,建议使用 <see cref="Class.InvokeMethod(string, Type[], object[])"/> 系列重载,获取指定的重载方法 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object InvokeMethod(String name, params object[] parameters)
        {
            return ClassType.GetRuntimeMethods().First(m => m.Name == name).Invoke(TargetObject, parameters);
        }

        /// <summary>
        /// 根据方法名执行一个方法,并提供相应的参数,而且将返回值转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高,对于重载方法,建议使用 <see cref="Class.InvokeMethod{T}(string, Type[], object[])"/> 系列重载,获取指定的重载方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T InvokeMethod<T>(String name, params object[] parameters)
        {
            return (T)ClassType.GetRuntimeMethods().First(m => m.Name == name).Invoke(TargetObject, parameters);
        }

        /// <summary>
        /// 根据方法名与参数类型执行一个方法,并提供相应的参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="parametersType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object InvokeMethod(String name, Type[] parametersType, params object[] parameters)
        {
            return ClassType.GetRuntimeMethod(name, parametersType).Invoke(TargetObject, parameters);
        }

        /// <summary>
        /// 根据方法名与参数类型执行一个方法,并提供相应的参数,而且将返回值转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="parametersType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T InvokeMethod<T>(String name, Type[] parametersType, params object[] parameters)
        {
            return (T)ClassType.GetRuntimeMethod(name, parametersType).Invoke(TargetObject, parameters);
        }

        /// <summary>
        /// 根据方法名执行一个泛型方法,并提供相应的参数,对于重载方法,建议使用 <see cref="Class.InvokeMethodWithGeneric(Type[], string, Type[], object[])"/> 系列重载,获取指定的重载方法 
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object InvokeMethodWithGeneric(Type[] genericType, String name, params object[] parameters)
        {
            return ClassType.GetRuntimeMethods().First(m => m.Name == name).MakeGenericMethod(genericType).Invoke(TargetObject, parameters);
        }

        /// <summary>
        /// 根据方法名执行一个泛型方法,并提供相应的参数,而且将返回值转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高,对于重载方法,建议使用 <see cref="Class.InvokeMethodWithGeneric{T}(Type[], string, Type[], object[])"/> 系列重载,获取指定的重载方法 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="genericType"></param>
        /// <param name="name"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T InvokeMethodWithGeneric<T>(Type[] genericType, String name, params object[] parameters)
        {
            return (T)ClassType.GetRuntimeMethods().First(m => m.Name == name).MakeGenericMethod(genericType).Invoke(TargetObject, parameters);
        }

        /// <summary>
        /// 根据方法名与参数类型执行一个泛型方法,并提供相应的参数
        /// </summary>
        /// <param name="genericType"></param>
        /// <param name="name"></param>
        /// <param name="parametersType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public object InvokeMethodWithGeneric(Type[] genericType, String name, Type[] parametersType, params object[] parameters)
        {
            return ClassType.GetRuntimeMethod(name, parametersType).MakeGenericMethod(genericType).Invoke(TargetObject, parameters);
        }

        /// <summary>
        /// 根据方法名与参数类型执行一个泛型方法,并提供相应的参数,而且将返回值转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="genericType"></param>
        /// <param name="name"></param>
        /// <param name="parametersType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T InvokeMethodWithGeneric<T>(Type[] genericType, String name, Type[] parametersType, params object[] parameters)
        {
            return (T)ClassType.GetRuntimeMethod(name, parametersType).MakeGenericMethod(genericType).Invoke(TargetObject, parameters);
        }

        /// <summary>
        /// 根据字段名获取字段值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Object GetField(String name)
        {
            return ClassType.GetRuntimeField(name).GetValue(TargetObject);
        }

        /// <summary>
        /// 根据字段名获取字段,并转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        public T GetField<T>(String name)
        {
            return (T)ClassType.GetRuntimeField(name).GetValue(TargetObject);
        }

        /// <summary>
        /// 根据字段名,设置字段值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetField(String name, Object value)
        {
            ClassType.GetRuntimeField(name).SetValue(TargetObject, value);
        }

        /// <summary>
        /// 根据事件名,自动添加一个委托到一个事件中,注意,该事件的签名必须是无返回值,并且拥有两个任意参数,会返回自动生成的对应类型的委托,用于使用 <see cref="Class.RemoveEventHandler(string, Delegate)"/> 移除该委托
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        /// <returns>自动转换的委托,可以在之后使用 <see cref="Class.RemoveEventHandler(string, Delegate)"/> 移除该委托</returns>
        public Delegate AddEventHandler(String name, Action<Object, Object> handler)
        {
            var eventInfo = ClassType.GetRuntimeEvent(name);
            var invokeMethod = eventInfo.EventHandlerType.GetRuntimeMethods().First(m => m.Name == "Invoke");
            var parameters = invokeMethod.GetParameters();
            var returnParameter = invokeMethod.ReturnParameter;

            if (parameters.Length == 2 && returnParameter.ParameterType == typeof(void))
            {
                Class creatorClass = new Class(typeof(EventHandlerCreator<,>));
                creatorClass.MakeGenericType(parameters.Select(i => i.ParameterType).ToArray());
                creatorClass.TargetObject = creatorClass.CreateInstance();
                creatorClass.SetProperty("Handler", handler);

                var triggleMethod = creatorClass.ClassType.GetRuntimeMethods().First(m => m.Name == "Triggle");
                var eventDelegate = triggleMethod.CreateDelegate(eventInfo.EventHandlerType, creatorClass.TargetObject);

                EventRegister Register = new EventRegister(eventInfo, TargetObject);
                Register.RegisteEvent(eventDelegate);

                return eventDelegate;
            }
            return null;
        }

        /// <summary>
        /// 根据事件名,添加一个指定的委托类型到事件中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void AddEventHandler<T>(String name,T handler)
        {
            var eventInfo = ClassType.GetRuntimeEvent(name);
            WindowsRuntimeMarshal.AddEventHandler<T>((h => (EventRegistrationToken)eventInfo.AddMethod.Invoke(TargetObject, new object[] { h })), t => eventInfo.RemoveMethod.Invoke(TargetObject, new object[] { t }), handler);
        }

        /// <summary>
        /// 根据事件名,从事件中移除指定的委托类型的一个委托
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void RemoveEventHandler<T>(String name, T handler)
        {
            var eventInfo = ClassType.GetRuntimeEvent(name);
            WindowsRuntimeMarshal.RemoveEventHandler<T>(t => eventInfo.RemoveMethod.Invoke(TargetObject, new object[] { t }), handler);
        }

        /// <summary>
        /// 根据事件名,从事件中移除一个委托
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void RemoveEventHandler(String name, Delegate handler)
        {
            var eventInfo = ClassType.GetRuntimeEvent(name);
            WindowsRuntimeMarshal.RemoveEventHandler(t => eventInfo.RemoveMethod.Invoke(TargetObject, new object[] { t }), handler);
        }

        /// <summary>
        /// 使用默认的无参数构造函数创建一个实例
        /// </summary>
        /// <returns></returns>
        public Object CreateInstance()
        {
            return Activator.CreateInstance(ClassType);
        }

        /// <summary>
        /// 使用指定参数的构造函数创建一个实例
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Object CreateInstance(params Object[] parameters)
        {
            return Activator.CreateInstance(ClassType,parameters);
        }

        /// <summary>
        /// 使用默认的无参数构造函数创建一个实例,并转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateInstance<T>()
        {
            return (T)Activator.CreateInstance(ClassType);
        }

        /// <summary>
        /// 使用指定参数的构造函数创建一个实例,并转换成指定类型的值,该重载只是为了转换方便,并没有性能上的提高
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public T CreateInstance<T>(params Object[] parameters)
        {
            return (T)Activator.CreateInstance(ClassType, parameters);
        }
    }
}
