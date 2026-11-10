using Dapper;
using DocumentManagementlibrary.Models;
using DocumentManagementlibrary.RequestModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DocumentManagementlibrary.DocumentRepository
{
    public class AddDocumentRepository
    {
        private readonly string connectionString;

        public AddDocumentRepository()
        {
            this.connectionString = "";// ADD THE CONNECTION STRING HERE
        }

        public int AddDocument(DocumentRequestModel model)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                return cn.ExecuteScalar<int>(
                    "dbo.SP_Document",
                    new
                    {
                        Mode = "INSERT_DOCUMENT",
                        model.OriginalFileName,
                        model.StoredFileName,
                        model.ContentType,
                        model.FileSize
                    },
                    commandTimeout: 30,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public DocumentModel? GetDocumentById(int id)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                return cn.QueryFirstOrDefault<DocumentModel>(
                    "dbo.SP_Document",
                    new { Mode = "GET_BY_ID", Id = id },
                    commandTimeout: 30,
                    commandType: CommandType.StoredProcedure);
            }
        }

        public List<DocumentModel> GetAllDocuments()
        {
            using (var cn = new SqlConnection(connectionString))
            {
                return cn.Query<DocumentModel>(
                    "dbo.SP_Document",
                    new { Mode = "GET_ALL" },
                    commandTimeout: 30,
                    commandType: CommandType.StoredProcedure).ToList();
            }
        }
        public void DeleteDocument(int id)
        {
            using (var cn = new SqlConnection(connectionString))
            {
                 cn.Execute(
                    "dbo.SP_Document",
                    new { Mode = "DELETE_DOCUMENT", Id = id },
                    commandTimeout: 30,
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}