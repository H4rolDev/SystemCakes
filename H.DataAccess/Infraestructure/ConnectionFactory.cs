using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H.DataAccess.Infraestructure
{
    public class ConnectionFactory : IConnectionFactory
    {
        // Obtener la cadena de conexion de appsettings.json
        private string _connectionString;

        public ConnectionFactory(string connectionString)
        {
            _connectionString = connectionString;
        }


        // Crear la conexion
        // retornar la conexion creada
        // Error: capturar el error y guardar en la ruta local : log.txt
        public IDbConnection GetConnection
        {
            get
            {
                try
                {

                    DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
                    var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

                    var connection = factory.CreateConnection();
                    if (connection != null)
                    {
                        connection.ConnectionString = _connectionString;
                        connection.Open();
                        return connection;
                    }

                    return null;

                }
                catch (Exception ex)
                {

                    //System.IO.File()

                    return null;
                }

            }
        }

        public IDbConnection CreateConnection()
        {
            try
            {
                DbProviderFactories.RegisterFactory("System.Data.SqlClient", SqlClientFactory.Instance);
                var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");

                var connection = factory.CreateConnection();
                if (connection != null)
                {
                    connection.ConnectionString = _connectionString;
                    connection.Open();
                    return connection;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
