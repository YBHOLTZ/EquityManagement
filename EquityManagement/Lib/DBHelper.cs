using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace EquityManagement.Model
{
    public class DBHelper
    {        
        public string Connection_string { get; private set; }

        public DBHelper(string connection_string)
        {
            this.Connection_string = connection_string;
        }

        SqlConnection openConnection()
        {
            SqlConnection sqlConnection = new SqlConnection(Connection_string);
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();
            return sqlConnection;
        }

        int ExecuteCommand(string query)
        {
            int affecteds = 0;
            using (SqlConnection sqlConnection = this.openConnection())
            {
                SqlCommand command = new SqlCommand(query, sqlConnection);
                affecteds = command.ExecuteNonQuery();                
            }
            return affecteds;
        }

        public List<T> ExecuteQuery<T>(string query, object[] parameters = null)
        {
            using (SqlConnection sqlConnection = this.openConnection())
            {                
                SqlCommand command = new SqlCommand(query, sqlConnection);                

                if(parameters != null)
                {
                    SqlParameter[] sqlParameters = buildParameters(parameters);
                    command.Parameters.AddRange(sqlParameters);
                }

                SqlDataReader sqlDataReader = command.ExecuteReader();

                if (!typeof(T).IsClass)
                    return ReadAllDataAsSimpleType<T>(sqlDataReader);

                return ReadAllData<T>(sqlDataReader);
            }
        }

        List<T> ReadAllDataAsSimpleType<T>(SqlDataReader sqlDataReader)
        {
            List<T> simpleTypes = new List<T>();
            while (sqlDataReader.Read())
            {
                simpleTypes.Add((T)sqlDataReader.GetValue(0));
            }
            return simpleTypes;
        }

        public int ExecuteNonQuery(string query, SqlParameter[] sqlParameters)
        {
            try
            {
                using (SqlConnection sqlConnection = this.openConnection())
                {
                    SqlCommand command = new SqlCommand(query, sqlConnection);

                    command.Parameters.AddRange(sqlParameters);
                    return command.ExecuteNonQuery();
                }
            }catch(Exception e)
            {
                throw new Exception(e.Message, e);
            }
        }


        List<T> ReadAllData<T>(SqlDataReader sqlDataReader)
        {
            List<T> instances = new List<T>();
            Type type = typeof(T);        
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>(type.GetProperties());

            while (sqlDataReader.Read())
            {
                T instance = buildInstance<T>(sqlDataReader,propertyInfos);
                instances.Add(instance);
            }
            return instances;
        }

        //TODO: Deveria ser atribuição to Entity.
        T buildInstance<T>(IDataRecord dataRecord, List<PropertyInfo> propertyInfos)
        {
            T instance = Activator.CreateInstance<T>();
            int len = dataRecord.FieldCount;
            for (int i = 0; i < len; i++)
            {
                string field_name = dataRecord.GetName(i).ToLower();
                if (propertyInfos.Find(p => p.Name.ToLower().Equals(field_name)) != null)
                {
                    object value = dataRecord.GetValue(i);
                    instance.GetType().GetProperty(propertyInfos[i].Name).SetValue(instance, value);
                }
            }
            return instance;
        }

        SqlParameter[] buildParameters(object[] parameters)
        {
            SqlParameter[] sqlParameters = new SqlParameter[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                sqlParameters[0] = new SqlParameter($"P{i}", parameters[i]);
            }
            return sqlParameters;
        }

    }
}
