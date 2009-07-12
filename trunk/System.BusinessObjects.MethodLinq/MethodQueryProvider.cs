using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections;

namespace System.BusinessObjects.MethodLinq
{
    /// <summary>
    /// Based on code from: http://blogs.msdn.com/mattwar/archive/2007/07/31/linq-building-an-iqueryable-provider-part-ii.aspx
    /// </summary>
    public class MethodQueryProvider : IQueryProvider
    {
        object methodOwner;

        public MethodQueryProvider(object ownerClass)
        {
            if (ownerClass == null)
                throw new ArgumentNullException("ownerClass", "Method's owner class is null.");
            methodOwner = ownerClass;
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new MethodLinqQuery<S>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(MethodLinqQuery<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }
        S IQueryProvider.Execute<S>(Expression expression)
        {
            IEnumerator<S> rdr = ((IEnumerable<S>)this.Execute(expression)).GetEnumerator();
            if(rdr.MoveNext())
                return rdr.Current;
            return default(S);
        }
        object IQueryProvider.Execute(Expression expression)
        {
            IEnumerator rdr = ((IEnumerable)this.Execute(expression)).GetEnumerator();
            if (rdr.MoveNext())
                return rdr.Current;
            return null;
        }

        public string GetQueryText(Expression expression)
        {
            return new QueryTranslator(methodOwner.GetType()).Translate(expression);
        }

        public object Execute(Expression expression)
        {
            //Execute against owning class
            QueryTranslator trans = new QueryTranslator(methodOwner.GetType());
            trans.Translate(expression);
            List<object> methodParamList = BuildMethodParameters(trans);

            try
            {
                /** Invoke the target method with the parsed parameters **/
                object retval = trans.targetMethod.Invoke(methodOwner, methodParamList.ToArray());

                /** Begin Formatting results **/
                Type elementType = TypeSystem.GetElementType(trans.targetMethod.ReturnType);
                Type selectElementType = TypeSystem.GetElementType(expression.Type);
                Type genericListType = typeof(List<>).MakeGenericType(selectElementType);
                MethodInfo addObjects = genericListType.GetMethod("Add");
                var retList = Activator.CreateInstance(genericListType);

                if (elementType.GetInterface("IEnumerable") == null)
                {
                    if (trans.selectMethod != null)
                    {
                        Delegate selectMethod = trans.selectMethod.Compile();
                        ParameterInfo[] lambdaParams = selectMethod.Method.GetParameters();
                        if (lambdaParams.Length > 1)
                        {
                            if (elementType != lambdaParams[1].ParameterType) //put retval from method inside select type record from query
                            {
                                object selectRecord = Activator.CreateInstance(lambdaParams[1].ParameterType);
                                PropertyInfo retValPropertyInfo = selectRecord.GetType().GetProperty("RetVal");
                                retValPropertyInfo.SetValue(selectRecord, retval, null);
                                retval = selectRecord;
                            }
                        }
                        try
                        {
                            retval = selectMethod.DynamicInvoke(retval);
                        }
                        catch (Exception ex)
                        {
                            throw new MethodLinqException(string.Format("Lambda Expression Error: '{0}' while executing '{1}'", ex.InnerException.Message, trans.selectMethod), ex);
                        }
                    }
                    else if (elementType != selectElementType) //put retval from method inside select type record from query
                    {
                        object selectRecord = Activator.CreateInstance(selectElementType);
                        PropertyInfo retValPropertyInfo = selectElementType.GetProperty("RetVal");
                        retValPropertyInfo.SetValue(selectRecord, retval, null);
                        retval = selectRecord;
                    }
                    addObjects.Invoke(retList, new object[] { retval });
                }
                else
                {
                    if (retval == null)
                        retval = Activator.CreateInstance(elementType);
                    foreach (object o in ((IEnumerable)retval))
                    {
                        addObjects.Invoke(retList, new object[] { o });
                    }
                }
                return retList;
            }
            catch (TargetInvocationException ex)
            { //Bubble up the real error, not the reflection invoke error
                if (ex.InnerException != null)
                    throw ex.InnerException;
                else throw ex;
            }
        }

        private static List<object> BuildMethodParameters(QueryTranslator query)
        {
            //holds the index of the parameter and parameter value
            Dictionary<int, object> paramList = new Dictionary<int, object>();
            ParameterInfo[] targetMethodParameters = query.targetMethod.GetParameters();

            foreach (MethodParamValue mpi in query.parsedMethodParameters)
            {
                object pVal = null;
                int index = -1;
                bool needsInsert = true;
                foreach (ParameterInfo info in targetMethodParameters)
                {
                    if (mpi.Param == info)
                    {
                        //Translate this expression into value, for now it only supports convertable types
                        object resolvedQueryValue = null;
                        index = mpi.Param.Position;
                        if (!string.IsNullOrEmpty(mpi.QueryVariableName) || ((Expression)mpi.Value).NodeType == ExpressionType.Constant)
                            pVal = resolvedQueryValue = QueryTranslator.ResolveToValue(mpi.Value as Expression, mpi.QueryVariableName);
                        else if (mpi.Value is MethodCallExpression)
                            pVal = resolvedQueryValue = QueryTranslator.ResolveToValue(mpi.Value as MethodCallExpression);
                        else
                            throw new NotSupportedException(string.Format("Unable to resolve the value of {0}", mpi.Value.ToString()));

                        if (resolvedQueryValue is IConvertible && mpi.Param.ParameterType.IsAssignableFrom(typeof(IConvertible)))
                        {
                            pVal = resolvedQueryValue = Convert.ChangeType(resolvedQueryValue, mpi.Param.ParameterType);
                        }
                        else if (mpi.Param.ParameterType.IsArray)
                        {
                            //create an item
                            if (!paramList.ContainsKey(index))
                            {
                                Array ar = Array.CreateInstance(mpi.Param.ParameterType.GetElementType(), 1);
                                object itemInst = null;
                                if (pVal != null && pVal.GetType() == mpi.Param.ParameterType.GetElementType())
                                    itemInst = pVal;
                                else
                                    itemInst = Activator.CreateInstance(mpi.Param.ParameterType.GetElementType());
                                ar.SetValue(itemInst, 0);
                                paramList.Add(index, ar);
                            }
                            else if (pVal != null && pVal.GetType() == mpi.Param.ParameterType.GetElementType())
                                throw new NotSupportedException(string.Format("Multiple array parameters are not supported. Error on {0}", mpi.Param.Name));
                            needsInsert = false;
                        }
                        else
                        {
                            pVal = resolvedQueryValue;
                        }

                        //This means a parameter object needs to be build
                        //eg. FirstObject.PropertyId = 123
                        if (mpi.ParamPath.Count > 0)
                        {
                            object tempParamObj = null;
                            object lastParamObj = null;
                            object tempValue = null;
                            needsInsert = !paramList.TryGetValue(index, out tempParamObj);
                            if (tempParamObj == null)
                                tempParamObj = Activator.CreateInstance(mpi.ParamPath[mpi.ParamPath.Count - 1].ReflectedType);
                            //this is the value to be passed into the function
                            pVal = lastParamObj = tempParamObj;
                            if (tempParamObj.GetType().IsArray)
                                lastParamObj = tempParamObj = ((Array)tempParamObj).GetValue(0);

                            //build the property values:
                            for (int i = mpi.ParamPath.Count-1; i >= 0; i--)
                            { //begin to walk along the Propert && Field path to set the required value, initiallising as required
                                if (TypeSystem.GetPropertyType(mpi.ParamPath[i]).IsPrimitive || i == 0) //set value and be done
                                    QueryTranslator.SetValue(mpi.ParamPath[i], lastParamObj, resolvedQueryValue); 
                                else if (i > 0)
                                {
                                    tempParamObj = QueryTranslator.GetValue(mpi.ParamPath[i], lastParamObj);
                                    if (tempParamObj == null)
                                    { //if the property is null, create an instance and conintue
                                        tempValue = Activator.CreateInstance(TypeSystem.GetPropertyType(mpi.ParamPath[i]));
                                        QueryTranslator.SetValue(mpi.ParamPath[i], lastParamObj, tempValue);
                                        lastParamObj = tempValue;
                                    }
                                    else //object exists, move another level down
                                        lastParamObj = tempParamObj;
                                }
                            }
                        }

                        break;
                    }
                }
                if (index > -1 && needsInsert)
                    paramList.Add(index, pVal);
            }
            List<object> passInList = new List<object>();
            for (int i = 0; i < targetMethodParameters.Length; i++)
            {
                object value = null;
                if (paramList.ContainsKey(i))
                    value = paramList[i];
                passInList.Add(value);
            }
            return passInList;
        }
    }
}
