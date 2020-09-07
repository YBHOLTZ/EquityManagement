using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using System.Data;

namespace EquityManagement.Model
{
    public class YboEntityFramework
    {
        DBHelper _dbHelper = null;

        public YboEntityFramework(string connectionString)
        {
            _dbHelper = new DBHelper(connectionString);
        }

        public int Add<T>(T entity)
        {
            Type type = typeof(T);
            string entityName = type.Name;

            List<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());

            PropertyInfo primaryKeyProperty = getPrimaryKeyProperty(type);
            properties.RemoveAll(i => i.Name.ToLower().Equals(primaryKeyProperty.Name.ToLower()));

            Type typeInstance = entity.GetType();

            string query = $@"INSERT INTO {entityName} ({properties[0].Name.ToUpper()}";          
            object first_value = typeInstance.GetProperty(properties[0].Name).GetValue(entity);
            string values_query = "VALUES (@P0";

            SqlParameter[] sqlParameters = new SqlParameter[properties.Count];
            sqlParameters[0] = new SqlParameter("@P0", first_value);

            for (int i = 1; i < properties.Count; i++)
            {
                object value = typeInstance.GetProperty(properties[i].Name).GetValue(entity);
                string parameterName = $"@P{i}";
                sqlParameters[i] = new SqlParameter(parameterName, value);

                query += $",{properties[i].Name.ToUpper()}";
                values_query += $",{parameterName}";
            }

            string full_query = $"{query}) {values_query})";

            return _dbHelper.ExecuteNonQuery(full_query, sqlParameters);
            
        }

        public int Update<T>(T entity)
        {
            Type type = typeof(T);
            string entityName = type.Name;

            List<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());

            PropertyInfo primaryKeyProperty = getPrimaryKeyProperty(type);
            properties.RemoveAll(i => i.Name.ToLower().Equals(primaryKeyProperty.Name.ToLower()));

            string query = $@"UPDATE {entityName} SET {properties[0].Name} = @P0";

            object first_value = type.GetProperty(properties[0].Name).GetValue(entity);
            List<SqlParameter> sqlParameters = new List<SqlParameter>() { new SqlParameter("@P0", first_value) };

            for (int i = 1; i < properties.Count; i++)
            {
                object value = type.GetProperty(properties[i].Name).GetValue(entity);
                string paramName = $@"@P{i}";
                query += $@",{properties[i].Name} = {paramName}";

                sqlParameters.Add(new SqlParameter(paramName, value));
            }

            string where = $@" WHERE {primaryKeyProperty.Name} = @P{properties.Count}";
            sqlParameters.Add(new SqlParameter($"@P{properties.Count}", primaryKeyProperty.GetValue(entity)));

            string full_query = $"{query}{where}";

            return _dbHelper.ExecuteNonQuery(full_query, sqlParameters.ToArray());
        }

        public int Remove<T>(T entity)
        {
            Type type = typeof(T);
            string entityName = type.Name;

            List<PropertyInfo> properties = new List<PropertyInfo>(type.GetProperties());
            PropertyInfo primaryKeyProperty = getPrimaryKeyProperty(type);

            string query = $@"DELETE FROM {entityName} WHERE {primaryKeyProperty.Name} = @P0";
            SqlParameter sqlParameter = new SqlParameter("@P0", primaryKeyProperty.GetValue(entity));

            return _dbHelper.ExecuteNonQuery(query, new SqlParameter[] { sqlParameter });
        }

        public List<T> All<T>()
        {
            Type type = typeof(T);
            string entityName = type.Name;

            string query = $"SELECT * FROM {entityName}";

            List<T> ls = _dbHelper.ExecuteQuery<T>(query);
            return ls;
        }

        public T Find<T>(object primaryKey_value)
        {
            Type type = typeof(T);
            string query = $@"SELECT * FROM {type.Name} WHERE ";

            PropertyInfo primaryKeyProperty = getPrimaryKeyProperty(type);

            if(primaryKeyProperty == null)
            {
                throw new Exception("Primary key property not found");
            }
            query += $"{primaryKeyProperty.Name} = @P0";

            List<T> items = _dbHelper.ExecuteQuery<T>(query, new object[] { primaryKey_value });

            if (items.Count < 1) return Activator.CreateInstance<T>();

            return items[0];
        }

        public List<T> FreeQuery<T>(string query, params object[] parameters)
        {
            return _dbHelper.ExecuteQuery<T>(query, parameters);
        }

        PropertyInfo getPrimaryKeyProperty(Type type)
        {            
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>(type.GetProperties());
            for (int i = 0; i < propertyInfos.Count; i++)
            {
                PropertyInfo propertyInfo = propertyInfos[i];
                IEnumerable<CustomAttributeData> customAttributes = propertyInfo.CustomAttributes;
                CustomAttributeData customAttributeData = customAttributes.FirstOrDefault(ac => ac.AttributeType.Equals(typeof(Lib.YboEntityAnnotations.PrimaryKeyAttribute)));
                if (customAttributeData != null) return propertyInfo;

                if (propertyInfos[i].Name.ToLower().StartsWith("id"))
                    return propertyInfo;
            }
            return null;
        }

    }
}
