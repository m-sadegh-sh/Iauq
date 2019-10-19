using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services.Impl
{
    public class FileService : IFileService
    {
        private readonly IDbSet<File> _files;
        private readonly IUnitOfWork _unitOfWork;

        public FileService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _files = _unitOfWork.Set<File>();
        }

        #region IFileService Members

        public File GetFileById(int fileId)
        {
            return _files.Find(fileId);
        }

        public File GetFileByGuid(Guid guid)
        {
            return _files.FirstOrDefault(f => f.Guid == guid);
        }

        public void SaveFile(File file)
        {
            _files.Add(file);
        }

        public void DeleteFile(File file)
        {
            _files.Remove(file);
        }

        public IQueryable<File> GetAllFilesByModes(AccessMode[] modes)
        {
            List<short> t = modes.Cast<short>().ToList();

            return _files.Where(c => t.Contains(c.AccessModeShort));
        }

        public IQueryable<File> GetAllFiles()
        {
            return _files.Where(f => !f.ParentId.HasValue);
        }

        public IQueryable<File> GetAllFilesByParentId(int parentId)
        {
            return _files.Where(f => f.ParentId == parentId);
        }

        #endregion
    }
}