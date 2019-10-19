using System;
using System.Collections.Generic;
using System.Linq;
using Iauq.Core.Domain;

namespace Iauq.Data.Services
{
    public interface IFileService
    {
        IQueryable<File> GetAllFiles();
        IQueryable<File> GetAllFilesByModes(AccessMode[] modes);
        IQueryable<File> GetAllFilesByParentId(int parentId);
        File GetFileById(int fileId);
        File GetFileByGuid(Guid guid);
        void SaveFile(File file);
        void DeleteFile(File file);
    }
}