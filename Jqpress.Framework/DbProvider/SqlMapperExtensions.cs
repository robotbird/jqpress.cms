using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Collections.Concurrent;
using System.Reflection.Emit;
using System.Threading;
using System.Runtime.CompilerServices;

namespace Jqpress.Framework.DbProvider
{

    public static class SqlMapperExtensions
    {
        public interface IProxy
        {
            bool IsDirty { get; set; }
        }

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary = new Dictionary<string, ISqlAdapter>() {
																							{"sqlconnection", new SqlServerAdapter()},
																							{"npgsqlconnection", new PostgresAdapter()},
                                                                                            {"oracleconnection", new OracleAdapter()},
                                                                                            {"mysqlconnection", new MySqlAdapter()}
																						};

        private static IEnumerable<PropertyInfo> KeyPropertiesCache(Type type)
        {

            IEnumerable<PropertyInfo> pi;
            if (KeyProperties.TryGetValue(type.TypeHandle, out pi))
            {
                return pi;
            }

            var allProperties = TypePropertiesCache(type);
            var keyProperties = allProperties.Where(p => p.GetCustomAttributes(true).Any(a => a is KeyAttribute)).ToList();

            if (keyProperties.Count == 0)
            {
                var idProp = allProperties.Where(p => p.Name.ToLower() == "id").FirstOrDefault();
                if (idProp != null)
                {
                    keyProperties.Add(idProp);
                }
            }

            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }
        private static IEnumerable<PropertyInfo> TypePropertiesCache(Type type)
        {
            IEnumerable<PropertyInfo> pis;
            if (TypeProperties.TryGetValue(type.TypeHandle, out pis))
            {
                return pis;
            }

            var properties = type.GetProperties().Where(IsWriteable);
            TypeProperties[type.TypeHandle] = properties;
            return properties;
        }

        public static bool IsWriteable(PropertyInfo pi)
        {
            object[] attributes = pi.GetCustomAttributes(typeof(WriteAttribute), false);
            if (attributes.Length == 1)
            {
                WriteAttribute write = (WriteAttribute)attributes[0];
                return write.Write;
            }
            return true;
        }

        /// <summary>
        /// Returns a single entity by a single id from table "Ts". T must be of interface type. 
        /// Id must be marked with [Key] attribute.
        /// Created entity is tracked/intercepted for changes and used by the Update() extension. 
        /// </summary>
        /// <typeparam name="T">Interface type to create and populate</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="id">Id of the entity to get, must be marked with [Key] attribute</param>
        /// <returns>Entity of T</returns>
        public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            ISqlAdapter adapter = GetFormatter(connection);

            var type = typeof(T);
            string sql;
            if (!GetQueries.TryGetValue(type.TypeHandle, out sql))
            {
                var keys = KeyPropertiesCache(type);
                if (keys.Count() > 1)
                    throw new DataException("Get<T> only supports an entity with a single [Key] property");
                if (keys.Count() == 0)
                    throw new DataException("Get<T> only supports en entity with a [Key] property");

                var onlyKey = keys.First();

                var name = GetTableName(type);

                // TODO: pluralizer 
                // TODO: query information schema and only select fields that are both in information schema and underlying class / interface 
                sql = "select * from " + name + " where " + onlyKey.Name + " = " + adapter.PreParamName + "id";
                GetQueries[type.TypeHandle] = sql;
            }

            var dynParms = new DynamicParameters();
            dynParms.Add(adapter.PreParamName + "id", id);
           
            T obj = null;

            if (type.IsInterface)
            {
                var res = connection.Query(sql, dynParms).FirstOrDefault() as IDictionary<string, object>;

                if (res == null)
                    return (T)((object)null);

                obj = ProxyGenerator.GetInterfaceProxy<T>();

                foreach (var property in TypePropertiesCache(type))
                {
                    var val = res[property.Name];
                    property.SetValue(obj, val, null);
                }

                ((IProxy)obj).IsDirty = false;   //reset change tracking and return
            }
            else
            {
                obj = connection.Query<T>(sql, dynParms, transaction: transaction, commandTimeout: commandTimeout).FirstOrDefault();
            }
            return obj;
        }

        private static string GetTableName(Type type)
        {
            string name;
            if (!TypeTableName.TryGetValue(type.TypeHandle, out name))
            {
                name = type.Name;// +"s";
                if (type.IsInterface && name.StartsWith("I"))
                    name = name.Substring(1);

                //NOTE: This as dynamic trick should be able to handle both our own Table-attribute as well as the one in EntityFramework 
                var tableattr = type.GetCustomAttributes(false).Where(attr => attr.GetType().Name == "TableAttribute").SingleOrDefault() as
                    dynamic;
                if (tableattr != null)
                    name = tableattr.Name;
                TypeTableName[type.TypeHandle] = name;
            }
            return name;
        }

        /// <summary>
        /// Inserts an entity into table "Ts" and returns identity id.
        /// </summary>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToInsert">Entity to insert</param>
        /// <returns>Identity of inserted entity</returns>
        public static long Insert<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {

            var type = typeof(T);
            ISqlAdapter adapter = GetFormatter(connection);

            var name = GetTableName(type);

            var sbColumnList = new StringBuilder(null);

            var allProperties = TypePropertiesCache(type);
            var keyProperties = KeyPropertiesCache(type).ToList();
            for (var i = keyProperties.Count - 1; i >= 0; i--)
            {
                if (null != keyProperties.ElementAt(i).GetValue(entityToInsert, null))
                    keyProperties.RemoveAt(i);
            }
            var allPropertiesExceptKey = allProperties.Except(keyProperties);

            for (var i = 0; i < allPropertiesExceptKey.Count(); i++)
            {
                var property = allPropertiesExceptKey.ElementAt(i);
                //edit by leo 为空的属性不赋值
                if (null == property.GetValue(entityToInsert, null))
                    continue;
                //sbColumnList.AppendFormat("[{0}]", property.Name);    //by liuliqiang  为解决数据库兼容性
                sbColumnList.AppendFormat("{0}, ", property.Name);
            }
            sbColumnList.Remove(sbColumnList.Length - 2, 2);

            var sbParameterList = new StringBuilder(null);
            for (var i = 0; i < allPropertiesExceptKey.Count(); i++)
            {
                var property = allPropertiesExceptKey.ElementAt(i);
                //edit by leo 为空的属性不赋值
                if (null == property.GetValue(entityToInsert, null))
                    continue;
                sbParameterList.AppendFormat("{1}{0}, ", property.Name, adapter.PreParamName);
            }
            sbParameterList.Remove(sbParameterList.Length - 2, 2);

            bool wasClosed = connection.State == ConnectionState.Closed;
            try
            {
                if (wasClosed) connection.Open();
                int id = adapter.Insert(connection, transaction, commandTimeout, name, sbColumnList.ToString(), sbParameterList.ToString(), keyProperties, entityToInsert);
                return id;
            }
            finally
            {
                if (wasClosed) connection.Close();
            }
        }

        /// <summary>
        /// Updates entity in table "Ts", checks if the entity is modified if the entity is tracked by the Get() extension.
        /// </summary>
        /// <typeparam name="T">Type to be updated</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToUpdate">Entity to be updated</param>
        /// <returns>true if updated, false if not found or not modified (tracked entities)</returns>
        public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            var proxy = entityToUpdate as IProxy;
            if (proxy != null)
            {
                if (!proxy.IsDirty) return false;
            }

            var type = typeof(T);

            var keyProperties = KeyPropertiesCache(type);
            if (!keyProperties.Any())
                throw new ArgumentException("Entity must have at least one [Key] property");

            ISqlAdapter adapter = GetFormatter(connection);
            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("update {0} set ", name);

            var allProperties = TypePropertiesCache(type);
            var nonIdProps = allProperties.Where(a => !keyProperties.Contains(a));

            for (var i = 0; i < nonIdProps.Count(); i++)
            {
                var property = nonIdProps.ElementAt(i);
                //edit by leo 为空的属性不修改值
                if (null == property.GetValue(entityToUpdate, null))
                    continue;
                sb.AppendFormat("{0} = {2}{1}, ", property.Name, property.Name, adapter.PreParamName);
            }
            sb.Remove(sb.Length - 2, 2);
            sb.Append(" where ");
            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var property = keyProperties.ElementAt(i);
                sb.AppendFormat("{0} = {2}{1}", property.Name, property.Name, adapter.PreParamName);
                if (i < keyProperties.Count() - 1)
                    sb.AppendFormat(" and ");
            }
            var updated = connection.Execute(sb.ToString(), entityToUpdate, commandTimeout: commandTimeout, transaction: transaction);
            return updated > 0;
        }

        /// <summary>
        /// Delete entity in table "Ts".
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="connection">Open SqlConnection</param>
        /// <param name="entityToDelete">Entity to delete</param>
        /// <returns>true if deleted, false if not found</returns>
        public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToDelete == null)
                throw new ArgumentException("Cannot Delete null Object", "entityToDelete");

            var type = typeof(T);

            var keyProperties = KeyPropertiesCache(type);
            if (keyProperties.Count() == 0)
                throw new ArgumentException("Entity must have at least one [Key] property");

            ISqlAdapter adapter = GetFormatter(connection);
            var name = GetTableName(type);

            var sb = new StringBuilder();
            sb.AppendFormat("delete from {0} where ", name);

            for (var i = 0; i < keyProperties.Count(); i++)
            {
                var property = keyProperties.ElementAt(i);
                sb.AppendFormat("{0} = {2}{1}", property.Name, property.Name, adapter.PreParamName);
                if (i < keyProperties.Count() - 1)
                    sb.AppendFormat(" and ");
            }
            var deleted = connection.Execute(sb.ToString(), entityToDelete, transaction: transaction, commandTimeout: commandTimeout);
            return deleted > 0;
        }

        /// <summary>
        /// 分页查询数据
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="connection">数据库连接</param>
        /// <param name="strSql">查询语句</param>
        /// <param name="orderBy">排序</param>
        /// <param name="pageIndex">查询页码</param>
        /// <param name="pageSize">每页查询条数</param>
        /// <param name="param">参数值</param>
        /// <param name="transaction"></param>
        /// <param name="buffered"></param>
        /// <param name="commandTimeout"></param>
        /// <param name="commandType"></param>
        /// <returns>分页数据</returns>
        /// comment by yepeng
        //public static PagedList<T> GetPagedListBySqlText<T>(this IDbConnection connection, string strSql, string orderBy, int pageIndex, int pageSize
        //     , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : class,new()
        //{
        //    ISqlAdapter adapter = GetFormatter(connection);
        //    return adapter.GetPagedListBySqlText<T>(connection, strSql, orderBy, pageIndex, pageSize, param as object, transaction, buffered, commandTimeout, commandType);
        //}

        /// <summary>
        /// 查询TOP数据
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="dins">数据库操作对象</param>
        /// <param name="strSql">查询语句</param>
        /// <param name="strSql">排序</param>
        /// <param name="pageSize">查询条数</param>
        /// <returns>数据</returns>
        public static IEnumerable<T> GetTopListBySqlText<T>(this IDbConnection connection, string strSql, string orderBy, int QueryCount
            , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            ISqlAdapter adapter = GetFormatter(connection);
            return adapter.GetTopListBySqlText<T>(connection, strSql, orderBy, QueryCount, param as object, transaction, buffered, commandTimeout, commandType);
        }

        public static ISqlAdapter GetFormatter(IDbConnection connection)
        {
            string name = connection.GetType().Name.ToLower();
            if (!AdapterDictionary.ContainsKey(name))
                return new SqlServerAdapter();
            return AdapterDictionary[name];
        }

        class ProxyGenerator
        {
            private static readonly Dictionary<Type, object> TypeCache = new Dictionary<Type, object>();

            private static AssemblyBuilder GetAsmBuilder(string name)
            {
                var assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName { Name = name },
                    AssemblyBuilderAccess.Run);       //NOTE: to save, use RunAndSave

                return assemblyBuilder;
            }

            public static T GetClassProxy<T>()
            {
                // A class proxy could be implemented if all properties are virtual
                //  otherwise there is a pretty dangerous case where internal actions will not update dirty tracking
                throw new NotImplementedException();
            }


            public static T GetInterfaceProxy<T>()
            {
                Type typeOfT = typeof(T);

                object k;
                if (TypeCache.TryGetValue(typeOfT, out k))
                {
                    return (T)k;
                }
                var assemblyBuilder = GetAsmBuilder(typeOfT.Name);

                var moduleBuilder = assemblyBuilder.DefineDynamicModule("SqlMapperExtensions." + typeOfT.Name); //NOTE: to save, add "asdasd.dll" parameter

                var interfaceType = typeof(SqlMapperExtensions.IProxy);
                var typeBuilder = moduleBuilder.DefineType(typeOfT.Name + "_" + Guid.NewGuid(),
                    TypeAttributes.Public | TypeAttributes.Class);
                typeBuilder.AddInterfaceImplementation(typeOfT);
                typeBuilder.AddInterfaceImplementation(interfaceType);

                //create our _isDirty field, which implements IProxy
                var setIsDirtyMethod = CreateIsDirtyProperty(typeBuilder);

                // Generate a field for each property, which implements the T
                foreach (var property in typeof(T).GetProperties())
                {
                    var isId = property.GetCustomAttributes(true).Any(a => a is KeyAttribute);
                    CreateProperty<T>(typeBuilder, property.Name, property.PropertyType, setIsDirtyMethod, isId);
                }

                var generatedType = typeBuilder.CreateType();

                //assemblyBuilder.Save(name + ".dll");  //NOTE: to save, uncomment

                var generatedObject = Activator.CreateInstance(generatedType);

                TypeCache.Add(typeOfT, generatedObject);
                return (T)generatedObject;
            }


            private static MethodInfo CreateIsDirtyProperty(TypeBuilder typeBuilder)
            {
                var propType = typeof(bool);
                var field = typeBuilder.DefineField("_" + "IsDirty", propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty("IsDirty",
                                               System.Reflection.PropertyAttributes.None,
                                               propType,
                                               new Type[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.NewSlot | MethodAttributes.SpecialName |
                                                    MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + "IsDirty",
                                             getSetAttr,
                                             propType,
                                             Type.EmptyTypes);
                var currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);
                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + "IsDirty",
                                             getSetAttr,
                                             null,
                                             new Type[] { propType });
                var currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ret);

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                var getMethod = typeof(SqlMapperExtensions.IProxy).GetMethod("get_" + "IsDirty");
                var setMethod = typeof(SqlMapperExtensions.IProxy).GetMethod("set_" + "IsDirty");
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);

                return currSetPropMthdBldr;
            }

            private static void CreateProperty<T>(TypeBuilder typeBuilder, string propertyName, Type propType, MethodInfo setIsDirtyMethod, bool isIdentity)
            {
                //Define the field and the property 
                var field = typeBuilder.DefineField("_" + propertyName, propType, FieldAttributes.Private);
                var property = typeBuilder.DefineProperty(propertyName,
                                               System.Reflection.PropertyAttributes.None,
                                               propType,
                                               new Type[] { propType });

                const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.Virtual |
                                                    MethodAttributes.HideBySig;

                // Define the "get" and "set" accessor methods
                var currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName,
                                             getSetAttr,
                                             propType,
                                             Type.EmptyTypes);

                var currGetIL = currGetPropMthdBldr.GetILGenerator();
                currGetIL.Emit(OpCodes.Ldarg_0);
                currGetIL.Emit(OpCodes.Ldfld, field);
                currGetIL.Emit(OpCodes.Ret);

                var currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                                             getSetAttr,
                                             null,
                                             new Type[] { propType });

                //store value in private field and set the isdirty flag
                var currSetIL = currSetPropMthdBldr.GetILGenerator();
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldarg_1);
                currSetIL.Emit(OpCodes.Stfld, field);
                currSetIL.Emit(OpCodes.Ldarg_0);
                currSetIL.Emit(OpCodes.Ldc_I4_1);
                currSetIL.Emit(OpCodes.Call, setIsDirtyMethod);
                currSetIL.Emit(OpCodes.Ret);

                //TODO: Should copy all attributes defined by the interface?
                if (isIdentity)
                {
                    var keyAttribute = typeof(KeyAttribute);
                    var myConstructorInfo = keyAttribute.GetConstructor(new Type[] { });
                    var attributeBuilder = new CustomAttributeBuilder(myConstructorInfo, new object[] { });
                    property.SetCustomAttribute(attributeBuilder);
                }

                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                var getMethod = typeof(T).GetMethod("get_" + propertyName);
                var setMethod = typeof(T).GetMethod("set_" + propertyName);
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
            }

        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TableAttribute : Attribute
    {
        public TableAttribute(string tableName)
        {
            Name = tableName;
        }
        public string Name { get; private set; }
    }

    // do not want to depend on data annotations that is not in client profile
    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class WriteAttribute : Attribute
    {
        public WriteAttribute(bool write)
        {
            Write = write;
        }
        public bool Write { get; private set; }
    }


    public interface ISqlAdapter
    {
        string PreParamName { get; }
        int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert);

        //comment by yepeng
        //PagedList<T> GetPagedListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int pageIndex, int pageSize
        //    , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : class,new();

        IEnumerable<T> GetTopListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int QueryCount
            , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null);
    }

    public class SqlServerAdapter : ISqlAdapter
    {
        public string PreParamName { get { return "@"; } }
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            string cmd = String.Format("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);

            int retVal = connection.Execute(cmd, entityToInsert, transaction: transaction, commandTimeout: commandTimeout);

            if (keyProperties.Any())
            {
                //NOTE: would prefer to use IDENT_CURRENT('tablename') or IDENT_SCOPE but these are not available on SQLCE
                var r = connection.Query("select @@IDENTITY id", transaction: transaction, commandTimeout: commandTimeout);
                int id = retVal = (int)r.First().id;

                keyProperties.First().SetValue(entityToInsert, id, null);
            }
            return retVal;
        }
        //comment by yepeng
        //public PagedList<T> GetPagedListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int pageIndex, int pageSize
        //    , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : class,new()
        //{
        //    if (string.IsNullOrEmpty(orderBy))
        //        throw new FormatException("排序字段不能为空");

        //    PagedList<T> pagedList = null;

        //    string totalRecordSql = string.Format(@"select count(1) from ({0}) [totalRecord] ", strSql);
        //    int recordCount = connection.Query<int>(totalRecordSql, param as object, transaction, buffered, commandTimeout, commandType).SingleOrDefault();
        //    if (recordCount > 0)
        //    {
        //        if (pageSize <= 0)
        //            pageSize = 100;
        //        int pageCount = (int)Math.Ceiling((decimal)recordCount / pageSize);
        //        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        //        pageIndex = pageIndex > pageCount ? pageCount : pageIndex;

        //        string pageDataSql = string.Format(@"select top ({3}) [Extent1].* from (select ROW_NUMBER() over ({1}) as [row_number], {0} ) as [Extent1] where [Extent1].[row_number] > {2} {1}",
        //            strSql.Substring(7), orderBy, (pageIndex - 1) * pageSize, pageSize);

        //        List<T> listT = connection.Query<T>(pageDataSql, param as object, transaction, buffered, commandTimeout, commandType).ToList();

        //        pagedList = new PagedList<T>(listT, pageIndex, pageSize, recordCount);
        //    }
        //    else
        //        pagedList = new PagedList<T>(new List<T>(), pageIndex, pageSize, 0);

        //    return pagedList;
        //}

        public IEnumerable<T> GetTopListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int QueryCount
            , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            string querySql = string.Format(@"select top ({2}) {0} {1}", strSql.Substring(7), orderBy, QueryCount);

            return connection.Query<T>(querySql, param as object, transaction, buffered, commandTimeout, commandType);
        }
    }

    public class PostgresAdapter : ISqlAdapter
    {
        public string PreParamName { get { return "@"; } }
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);

            // If no primary key then safe to assume a join table with not too much data to return
            if (!keyProperties.Any())
                sb.Append(" RETURNING *");
            else
            {
                sb.Append(" RETURNING ");
                bool first = true;
                foreach (var property in keyProperties)
                {
                    if (!first)
                        sb.Append(", ");
                    first = false;
                    sb.Append(property.Name);
                }
            }

            var results = connection.Query(sb.ToString(), entityToInsert, transaction: transaction, commandTimeout: commandTimeout);

            // Return the key by assinging the corresponding property in the object - by product is that it supports compound primary keys
            int id = 0;
            foreach (var p in keyProperties)
            {
                var value = ((IDictionary<string, object>)results.First())[p.Name.ToLower()];
                p.SetValue(entityToInsert, value, null);
                if (id == 0)
                    id = Convert.ToInt32(value);
            }
            return id;
        }

        //comment by yepeng
        //未经测试
        //public PagedList<T> GetPagedListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int pageIndex, int pageSize
        //    , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : class,new()
        //{
        //    PagedList<T> pagedList = null;

        //    string totalRecordSql = string.Format(@"select count(1) from ({0}) [totalRecord] ", strSql);
        //    int recordCount = connection.Query<int>(totalRecordSql, param as object, transaction, buffered, commandTimeout, commandType).SingleOrDefault();
        //    if (recordCount > 0)
        //    {
        //        if (pageSize <= 0)
        //            pageSize = 100;
        //        int pageCount = (int)Math.Ceiling((decimal)recordCount / pageSize);
        //        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        //        pageIndex = pageIndex > pageCount ? pageCount : pageIndex;

        //        string pageDataSql = string.Format(@"select top ({3}) [Extent1].* from (select ROW_NUMBER() over ({1}) as [row_number], {0} ) as [Extent1] where [Extent1].[row_number] > {2} {1}",
        //            strSql.Substring(7), orderBy, (pageIndex - 1) * pageSize, pageSize);

        //        List<T> listT = connection.Query<T>(pageDataSql, param as object, transaction, buffered, commandTimeout, commandType).ToList();

        //        pagedList = new PagedList<T>(listT, pageIndex, pageSize, recordCount);
        //    }
        //    else
        //        pagedList = new PagedList<T>(new List<T>(), pageIndex, pageSize, 0);

        //    return pagedList;
        //}
        ////未经测试
        public IEnumerable<T> GetTopListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int QueryCount
            , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            string querySql = string.Format(@"select top ({2}) {0} {1}", strSql.Substring(7), orderBy, QueryCount);

            return connection.Query<T>(querySql, param as object, transaction, buffered, commandTimeout, commandType);
        }
    }

    public class MySqlAdapter : ISqlAdapter
    {
        public string PreParamName { get { return "?"; } }
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            string cmd = String.Format("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);

            int retVal = connection.Execute(cmd, entityToInsert, transaction: transaction, commandTimeout: commandTimeout);

            if (keyProperties.Any())
            {
                var r = connection.Query("select @@IDENTITY id", transaction: transaction, commandTimeout: commandTimeout);
                int id = retVal = (int)r.First().id;
                keyProperties.First().SetValue(entityToInsert, id, null);
            }
            return retVal;
        }

        //comment by yepeng
        //public PagedList<T> GetPagedListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int pageIndex, int pageSize
        //    , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : class,new()
        //{
        //    PagedList<T> pagedList = null;

        //    string totalRecordSql = string.Format(@"select count(1) from ({0}) a ", strSql);
        //    int recordCount = connection.Query<int>(totalRecordSql, param as object, transaction, buffered, commandTimeout, commandType).SingleOrDefault();
        //    if (recordCount > 0)
        //    {
        //        if (pageSize <= 0)
        //            pageSize = 100;
        //        int pageCount = (int)Math.Ceiling((decimal)recordCount / pageSize);
        //        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        //        pageIndex = pageIndex > pageCount ? pageCount : pageIndex;

        //        string pageDataSql = string.Format(@"{0} {1} limit {2},{3}", strSql, orderBy, (pageIndex - 1) * pageSize, pageSize);

        //        List<T> listT = connection.Query<T>(pageDataSql, param as object, transaction, buffered, commandTimeout, commandType).ToList();

        //        pagedList = new PagedList<T>(listT, pageIndex, pageSize, recordCount);
        //    }
        //    else
        //        pagedList = new PagedList<T>(new List<T>(), pageIndex, pageSize, 0);

        //    return pagedList;
        //}

        public IEnumerable<T> GetTopListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int QueryCount
            , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            string querySql = string.Format(@"{0} {1} limit 0,{2}", strSql, orderBy, QueryCount);

            return connection.Query<T>(querySql, param as object, transaction, buffered, commandTimeout, commandType);
        }
    }

    public class OracleAdapter : ISqlAdapter
    {
        public string PreParamName { get { return ":"; } }
        public int Insert(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, String tableName, string columnList, string parameterList, IEnumerable<PropertyInfo> keyProperties, object entityToInsert)
        {
            string cmd = String.Format("insert into {0} ({1}) values ({2})", tableName, columnList, parameterList);

            return connection.Execute(cmd, entityToInsert, transaction: transaction, commandTimeout: commandTimeout);
        }

        //comment by yepeng
        //public PagedList<T> GetPagedListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int pageIndex, int pageSize
        //    , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null) where T : class,new()
        //{
        //    PagedList<T> pagedList = null;

        //    string totalRecordSql = string.Format(@"select count(1) from ({0}) a ", strSql);
        //    int recordCount = connection.Query<int>(totalRecordSql, param as object, transaction, buffered, commandTimeout, commandType).SingleOrDefault();
        //    if (recordCount > 0)
        //    {
        //        if (pageSize <= 0)
        //            pageSize = 100;
        //        int pageCount = (int)Math.Ceiling((decimal)recordCount / pageSize);
        //        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        //        pageIndex = pageIndex > pageCount ? pageCount : pageIndex;

        //        string pageDataSql = string.Format(@"select * from (select rownum iRow,a.* from ({0} {1}) a  where rownum<= {3} ) where iRow> {2}"
        //            , strSql, orderBy, (pageIndex - 1) * pageSize, pageIndex * pageSize);

        //        List<T> listT = connection.Query<T>(pageDataSql, param as object, transaction, buffered, commandTimeout, commandType).ToList();

        //        pagedList = new PagedList<T>(listT, pageIndex, pageSize, recordCount);
        //    }
        //    else
        //        pagedList = new PagedList<T>(new List<T>(), pageIndex, pageSize, 0);

        //    return pagedList;
        //}

        public IEnumerable<T> GetTopListBySqlText<T>(IDbConnection connection, string strSql, string orderBy, int QueryCount
            , dynamic param = null, IDbTransaction transaction = null, bool buffered = true, int? commandTimeout = null, CommandType? commandType = null)
        {
            string querySql = string.Format(@"select a.* from ({0} {1}) a where rownum<= {2} ", strSql, orderBy, QueryCount);

            return connection.Query<T>(querySql, param as object, transaction, buffered, commandTimeout, commandType);
        }
    }

}
