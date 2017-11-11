using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;


namespace Atom.Framework.Utility
{
    public class FileHelper
    {
        /// <summary>
        /// 复制文件夹（及文件夹下所有子文件夹和文件） 
        /// </summary>
        /// <param name="sourcePath">待复制的文件夹路径</param>
        /// <param name="destinationPath">目标路径</param>
        public static void CopyDirectory(String sourcePath, String destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                String destName = Path.Combine(destinationPath, fsi.Name);

                if (fsi is System.IO.FileInfo)          //如果是文件，复制文件 
                    File.Copy(fsi.FullName, destName);
                else                                    //如果是文件夹，新建文件夹，递归 
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }
        }

        public static FileStream OpenFile(string filepath)
        {
            FileStream file = null;
            try
            {
                if (!File.Exists(filepath))
                {
                    string directoryPath = filepath.Substring(0, filepath.LastIndexOf("\\"));
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                }
                file = File.Create(filepath);
            }
            catch (Exception ex)
            {
            }
            return file;
        }

        /// <summary>
        /// 复制子目录
        /// </summary>
        /// <param name="srcDir">
        /// The src dir.
        /// </param>
        /// <param name="desDir">
        /// The des dir.
        /// </param>
        /// <returns>
        /// 是否成功 <see cref="bool"/>.
        /// </returns>
        public static bool CopySubDirectory(string srcDir, string desDir)
        {
            try
            {
                if (!Directory.Exists(desDir))
                {
                    Directory.CreateDirectory(desDir);
                }
                DirectoryInfo srcinfo = new DirectoryInfo(srcDir);
                DirectoryInfo[] childDir = srcinfo.GetDirectories();//返回所有子目录
                FileInfo[] childFile = srcinfo.GetFiles();
                foreach (DirectoryInfo d in childDir)
                {
                    if (!CopySubDirectory(d.FullName, desDir + "\\" + d.Name))
                    {
                        throw new Exception("Move Directory Failed");
                    }
                }
                foreach (FileInfo f in childFile)
                {
                    f.CopyTo(desDir + "\\" + f.Name, true);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (Directory.Exists(desDir))
                {
                    DeleteSubDirectory(desDir);
                }
                return false;
            }

        }

        /// <summary>
        /// 移动文件夹 至一个新的路径
        /// copy文件夹 中的数据， 如果该文件夹中有文件正在打开不会报错 而是将数据复制过去，但是删除时该文件夹时会报错
        /// </summary>
        /// <param name="srcDir">源路径</param>
        /// <param name="desDir">新的路径，新的路径可以存在也可以不存在</param>
        public static void MoveSubDirectory(string srcDir, string desDir)
        {
            if (CopySubDirectory(srcDir, desDir))
            {
                if (Directory.Exists(srcDir))
                {
                    DeleteSubDirectory(srcDir);
                }
            }
        }

        /// <summary>
        /// 移动文件夹 至一个新的路径
        /// 直接移动文件夹 如果该文件夹中有文件正在打开 则在移动时报错
        /// </summary>
        /// <param name="srcDir">源路径</param>
        /// <param name="newDir">新的路径，新的路径不可存在</param>
        public static void MoveDirectory(string srcDir, string newDir)
        {
            Directory.Move(srcDir, newDir);
        }

        /// <summary>
        /// 文件夹名重命名
        /// </summary>
        /// <param name="srcDir">源路径</param>
        /// <param name="newName">新的文件夹名</param>
        public static void RenameDirectory(string srcDir, string newName)
        {
            var dir = Directory.GetParent(srcDir);
            var newDir = Path.Combine(dir.FullName, newName);
            Directory.Move(srcDir, newDir);
        }

        public static bool DeleteSubDirectory(string srcDir)
        {
            try
            {

                DirectoryInfo srcinfo = new DirectoryInfo(srcDir);
                if (!srcinfo.Exists)
                {
                    return true;
                }
                DirectoryInfo[] childDir = srcinfo.GetDirectories();
                FileInfo[] childFile = srcinfo.GetFiles();
                foreach (DirectoryInfo d in childDir)
                {
                    if (!DeleteSubDirectory(d.FullName))
                    {
                        throw new Exception("Delete Directory Failed");
                    }
                }
                foreach (FileInfo f in childFile)
                {
                    f.Delete();
                }
                srcinfo.Delete();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public static bool ReNameFile(string filepath, string newname)
        {
            int pos = filepath.LastIndexOf("\\");
            string fileroot = filepath.Substring(0, pos);
            string newfilepath = fileroot + "\\" + newname;
            if (File.Exists(filepath))
            {
                FileInfo fileinfo = new FileInfo(filepath);
                fileinfo.CopyTo(newfilepath, true);
                fileinfo.Delete();
            }
            return true;
        }

        /// <summary>
        /// 按照UTF8编码读取文件内容为字符串
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string GetFileText(string filepath)
        {
            using (StreamReader reader = new StreamReader(filepath, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 按照指定的编码读取文件内容为字符串
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string GetFileText(string filepath, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(filepath, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// 删除指定文件夹下的所有文件
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFilesOfDirectory(string path)
        {

            foreach (string item in Directory.GetFiles(path))
            {
                try
                {
                    File.Delete(item);
                }
                catch (Exception)
                {
                }
            }
            foreach (string item in Directory.GetDirectories(path))
            {
                DeleteFilesOfDirectory(item);
            }
        }


        public static byte[] FileToByte(string path)
        {
            byte[] image = null;
            using (FileStream buffer = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(buffer))
                {
                    image = br.ReadBytes((int)buffer.Length);
                    br.Close();
                    buffer.Close();
                }
            }
            return image;
        }

        /// <summary>
        /// 读取文件到内存流
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static MemoryStream FileToMemoryStream(string filePath)
        {
            using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                Byte[] buffer = new Byte[stream.Length];
                //从流中读取字节块并将该数据写入给定缓冲区buffer中
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                return new MemoryStream(buffer);
            }
        }

        public static void SaveByteToFile(byte[] data, string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                fileStream.Write(data, 0, data.Length);
                fileStream.Flush();
                fileStream.Close();
            }
        }

        /// <summary>
        /// 保存数据流到文件
        /// </summary>
        /// <param name="stream">要写到文件中的数据流</param>
        /// <param name="fileName">要保存的文件</param>
        public static void SaveStreamToFile(Stream stream, string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] bytes = new byte[1024];
                int readCount = 0;
                do
                {
                    readCount = stream.Read(bytes, 0, bytes.Length);
                    if (readCount > 0)
                    {
                        fileStream.Write(bytes, 0, readCount);
                    }
                } while (readCount == 0);
                fileStream.Flush();
                fileStream.Close();
            }
        }

        /// <summary>
        /// 按UTF8格式将字符串保存到文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="fileName"></param>
        public static void SaveStringToFile(string info, string fileName)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(file, Encoding.UTF8))
                {
                    sw.Write(info);
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 按指定编码格式将字符串保存到文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="fileName"></param>
        /// <param name="encoding"></param>
        public static void SaveStringToFile(string info, string fileName, Encoding encoding)
        {
            using (FileStream file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                using (StreamWriter sw = new StreamWriter(file, encoding))
                {
                    sw.Write(info);
                    sw.Flush();
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 删除快捷方式
        /// </summary>
        /// <param name="name">快捷方式的名称</param>
        public static void RemoveDesktopShortcut(string name)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\" + name + ".lnk";
            File.Delete(path);
        }

        /// <summary>
        /// 递归指定目录，针对每个符合扩展名的文件回调委托处理
        /// </summary>
        /// <param name="directoryString">要递归的目录</param>
        /// <param name="extensions">要求的文件扩展名数组，为空表示不限制扩展名,忽略大小写，需要包含“.”</param>
        /// <param name="doFileCallBack">回调委托</param>
        public static void RecursionFile(string directoryString, string[] extensions, DoFile doFileCallBack)
        {
            if (extensions != null)
                extensions = extensions.Where(m => m != null).Select(m => m.ToLower()).ToArray();
            foreach (string fileName in Directory.GetFiles(directoryString))
            {
                if (extensions == null || extensions.Contains(Path.GetExtension(fileName).ToLower()))
                    doFileCallBack(fileName);
            }
            foreach (string item in Directory.GetDirectories(directoryString))
                RecursionFile(item, extensions, doFileCallBack);
        }

        /// <summary>
        /// 遍历指定目录中文件(非递归，递归请使用RecursionFile)，针对每个符合扩展名的文件回调委托处理
        /// </summary>
        /// <param name="directoryString">要递归的目录</param>
        /// <param name="extensions">要求的文件扩展名数组，为空表示不限制扩展名,忽略大小写，需要包含“.”</param>
        /// <param name="doFileCallBack">回调委托</param>
        public static void ForEachFile(string directoryString, string[] extensions, DoFile doFileCallBack)
        {
            if (extensions != null)
                extensions = extensions.Where(m => m != null).Select(m => m.ToLower()).ToArray();
            foreach (string fileName in Directory.GetFiles(directoryString))
            {
                if (extensions == null || extensions.Contains(Path.GetExtension(fileName).ToLower()))
                    doFileCallBack(fileName);
            }
        }

        public delegate void DoFile(string fileFullName);

        public static byte[][] GetImages(string sourcePath)
        {
            List<byte[]> bytes = new List<byte[]>();
            DirectoryInfo info = new DirectoryInfo(sourcePath);

            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                if (fsi is System.IO.FileInfo)          //如果是文件，复制文件 
                {
                    byte[] newimage = FileToByte(fsi.FullName);
                    bytes.Add(newimage);
                }
                else                                    //如果是文件夹，新建文件夹，递归 
                {
                    byte[][] bts = GetImages(fsi.FullName);
                    bytes.AddRange(bytes);
                }
            }
            return bytes.ToArray();
        }

    }
}
