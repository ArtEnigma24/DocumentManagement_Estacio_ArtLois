using DocumentManagementlibrary.DocumentRepository;
using DocumentManagementlibrary.Models;
using DocumentManagementlibrary.RequestModel;
using System.Collections.Generic;

namespace DocumentManagementlibrary.Services
{
    public class AddDocumentServices
    {
        private readonly AddDocumentRepository _repo;

        public AddDocumentServices(AddDocumentRepository repo)
        {
            _repo = repo;
        }

        public int AddDocument(DocumentRequestModel model)
        {
            return _repo.AddDocument(model);
        }

        public DocumentModel? GetDocumentById(int id)
        {
            return _repo.GetDocumentById(id);
        }

        public List<DocumentModel> GetAllDocuments()
        {
            return _repo.GetAllDocuments();
        }

       
        public void DeleteDocument(int id)
        {
             _repo.DeleteDocument(id);
        }
    }
}