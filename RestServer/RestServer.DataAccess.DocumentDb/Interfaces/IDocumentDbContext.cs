using Microsoft.Azure.Documents;
using RestServer.Configuration.Models;
using RestServer.DataAccess.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.DataAccess.DocumentDb.Interfaces
{
    public interface IDocumentDbContext : IDataContext
    {
        bool IsInitialized { get; set; }

        Task InitializeAsync(DocumentDbSetting docDbSetting);

        IDocumentClient GetDocumentClient();

        string GetDatabase();

        string GetCollection(string entityName);
    }
}
