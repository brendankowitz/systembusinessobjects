using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection; 
using Iesi.Collections.Generic; 

namespace System.BusinessObjects.Data
{
    //found at:
    //http://forum.hibernate.org/viewtopic.php?t=959464&start=0&postdays=0&postorder=asc&highlight=

   
   public class SafeDataBindingList<T> : List<T> 
   { 
      private IList<T> _innerList; 

      private SafeDataBindingList(IList<T> list) : base(list.Count) 
      { 
         _innerList = list; 

         Castle.DynamicProxy.ProxyGenerator generator = new Castle.DynamicProxy.ProxyGenerator(); 

         foreach (T obj in list) 
         { 
            this.Add((T)generator.CreateClassProxy(typeof(T), new DataObjectInterceptor(obj))); 
         } 
      } 

      public IList<T> InnerList 
      { 
         get { return _innerList; } 
      } 

      /// <summary> 
      /// Creates a safe-wrapper around a list if needed for 
      /// </summary> 
      /// <param name="list"></param> 
      /// <returns></returns> 
      public static IList<T> Create(IList<T> list) 
      { 
         if (list.Count <= 1) 
            return list; 

         string t1 = ""; 
         string t2 = ""; 

         t1 = list[0].GetType().ToString(); 

         bool uniformList = true; 
         for (int i = 1; i < list.Count; i++) 
         { 
            t2 = list[i].GetType().ToString(); 
            if (t1 == t2) 
               continue; 
            else 
            { 
               uniformList = false; 
               break; 
            } 
         } 

         if (uniformList) 
            return list; 

         return new SafeDataBindingList<T>(list); 
      } 
   } 

   public class SafeDataBindingSet<T> : HashedSet<T> 
   { 
      private ISet<T> _innerSet; 

      private SafeDataBindingSet(ISet<T> set) : base() 
      { 
         _innerSet = set; 

         Castle.DynamicProxy.ProxyGenerator generator = new Castle.DynamicProxy.ProxyGenerator(); 

         foreach (T obj in set) 
         { 
            this.Add((T)generator.CreateClassProxy(typeof(T), new DataObjectInterceptor(obj))); 
         } 
      } 

      public ISet<T> InnerSet 
      { 
         get { return _innerSet; } 
      } 

      /// <summary> 
      /// Creates a safe-wrapper around a list if needed for 
      /// </summary> 
      /// <param name="list"></param> 
      /// <returns></returns> 
      public static ISet<T> Create(ISet<T> set) 
      { 
         if (set.Count <= 1) 
            return set; 

         string t1 = ""; 
         string t2 = ""; 

         bool uniformSet = true; 
         foreach (T obj in set) 
         { 
            if (t1 == "") 
            { 
               t1 = obj.GetType().ToString(); 
               continue; 
            } 

            t2 = obj.GetType().ToString(); 

            if (t1 == t2) 
               continue; 
            else 
            { 
               uniformSet = false; 
               break; 
            } 
         } 

         if (uniformSet) 
            return set; 

         return new SafeDataBindingSet<T>(set); 
      } 
   } 

   public class DataObjectInterceptor : Castle.DynamicProxy.IInterceptor 
   { 
      private static object _syncRoot = new object(); 
      private static Dictionary<string, MethodInfo> _methodCache = new Dictionary<string, MethodInfo>(); 
      private object _target = null; 

      public DataObjectInterceptor(object target) 
      { 
         _target = target; 
      } 

      public object Intercept(Castle.DynamicProxy.IInvocation invocation, params object[] args) 
      { 
         // it's possible that the type that the proxy was built on is not the type of 
         // the _target object.  If it is, simply invoke MethodInfo, if it's not, we have 
         // to find the appropriate MethodInfo on the unknown type. 
         if (invocation.Method.ReflectedType.ToString() == invocation.InvocationTarget.GetType().ToString()) 
         { 
            return invocation.Method.Invoke(_target, args); 
         } 
         else 
         { 
            MethodInfo method = GetMethod(invocation); 
            return method.Invoke(_target, args); 
         } 
      } 

      private MethodInfo GetMethod(Castle.DynamicProxy.IInvocation invocation) 
      { 

         Type targetType = _target.GetType(); 

         string methodKey = GetMethodKey(targetType, invocation.Method.Name); 

         if (_methodCache.ContainsKey(methodKey)) 
            return _methodCache[methodKey]; 
         else 
         { 
            MethodInfo methodInfo = null; 
            if (invocation.Method.Name.StartsWith("get_") || invocation.Method.Name.StartsWith("set_")) 
               methodInfo = GetPropertyMethod(targetType, invocation); 
            else 
            { 
               methodInfo = GetMethod(targetType, invocation.Method.Name); 
            } 

            lock (_syncRoot) 
            { 
               if (methodInfo != null && !_methodCache.ContainsKey(methodKey)) 
                  _methodCache.Add(methodKey, methodInfo); 
            } 

            return methodInfo; 
         } 
      } 

      private MethodInfo GetMethod(Type targetType, string methodName) 
      { 
         // search class hierarchy for method 
         Type type = targetType; 
         do 
         { 
            // Use DeclaredOnly to see if we can get the method directly from this type 
            MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly); 

            if (method != null) 
               return method; 

            type = type.BaseType; 
         } while (type != null); 

         return null; 
      } 

      private MethodInfo GetPropertyMethod(Type targetType, Castle.DynamicProxy.IInvocation invocation) 
      { 
         string propertyName = invocation.Method.Name.Split('_')[1]; 
         bool isGet = invocation.Method.Name.StartsWith("get_"); 

         Type type = targetType; 
         PropertyInfo propertyInfo = null; 

         do 
         { 
            propertyInfo = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly); 

            if (propertyInfo != null) 
               break; 

            type = type.BaseType; 

         } while (type != null); 

         if (propertyInfo == null) 
            return null; 

         if (isGet) 
            return propertyInfo.GetGetMethod(); 
         else 
            return propertyInfo.GetSetMethod(); 
      } 

      private string GetMethodKey(Type targetType, string methodName) 
      { 
         return targetType.ToString() + "." + methodName; 
      } 
   } 
}