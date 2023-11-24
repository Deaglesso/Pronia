using Pronia.Entities;

namespace Pronia.Utilities.Extensions
{
    public static class Helper
    {

        public static bool CheckFileSize(this IFormFile file,int MB)
        {
            if(file.Length > 1024 * 1024 * MB)
            {
                return false;
            }
            return true;
        }
        public static bool CheckFileType(this IFormFile file,string type) 
        {
            if (file.ContentType.Contains(type))
            {
                return true;
            }
            return false;

        }

        public static async Task<string> CreateFileAsync(this IFormFile file,string root, params string[] folders)
        {
            string nf = file.FileName.Substring(file.FileName.LastIndexOf("."));
            string filename = Guid.NewGuid().ToString()+ nf;

            string path = root;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }

            path = Path.Combine(path, filename);

            using (FileStream fs = new(path, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }
            return filename;
        }
        public static void Delete(this string filename,string root,params string[] folders)
        {
            string path = root;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            path = Path.Combine(path, filename);
            if(File.Exists(path))
            {
                File.Delete(path);
            }


        }
    }
}
