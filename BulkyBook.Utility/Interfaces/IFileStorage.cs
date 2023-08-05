using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility.Interfaces
{
    public interface IFileStorage
    {
        Task<string> EditFile(byte[] content, string extension, string container, string path, string type);
        Task DeleteFile(string path, string container);
        Task<string> SaveFile(byte[] content, string extension, string container, string type);
    }
}
