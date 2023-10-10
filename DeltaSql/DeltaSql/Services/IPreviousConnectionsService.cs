using System.Collections.Generic;

namespace DeltaSql.Services
{
    internal interface IPreviousConnectionsService
    {
        bool AddConnectionString(string connectionString);
        void AddNewConnectionString(string connectionString);
        string HidePasswordInConnectionString(string connectionString);
        void Initialize();
        List<string> ReadInPreviousConnectionStrings();
    }
}
