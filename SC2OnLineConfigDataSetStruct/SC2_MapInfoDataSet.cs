using System;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Windows;
using WH_CommonControlLibrary.Functionality.Common;
using WH_CommonControlLibrary.UIControl.UIWindow;

namespace SC2OnLineConfigDataSetStruct
{
    /// <summary>
    /// 星际争霸地图配置数据集
    /// </summary>
    partial class SC2_MapInfoDataSet
    {
        #region 声明
        /// <summary>
        /// 反序列化验证跳过版本
        /// </summary>
        private class UBinder : SerializationBinder
        {
            /// <summary>
            /// 变量类型映射
            /// </summary>
            /// <param name="assemblyName">程序集</param>
            /// <param name="typeName">类型名</param>
            /// <returns>返回值</returns>
            public override Type BindToType(string assemblyName, string typeName)
            {
                Assembly ass = Assembly.GetExecutingAssembly();
                return ass.GetType(typeName);
            }
        }

        /// <summary>
        /// SC2本地化语言类型枚举
        /// </summary>
        public enum EnumLanguage
        {
            /// <summary>
            /// 简体中文
            /// </summary>
            zhCN = 0x0804,
            /// <summary>
            /// 繁体中文
            /// </summary>
            zhTW = 0x0404,
            /// <summary>
            /// 英语美国
            /// </summary>
            enUS = 0x0409,
            /// <summary>
            /// 德语德国
            /// </summary>
            deDE = 0x0407,
            /// <summary>
            /// 西班牙语墨西哥
            /// </summary>
            esMX = 0x080A,
            /// <summary>
            /// 西班牙语西班牙
            /// </summary>
            esES = 0x0c0A,
            /// <summary>
            /// 法语法国
            /// </summary>
            frFR = 0x040C,
            /// <summary>
            /// 意大利语意大利
            /// </summary>
            itIT = 0x0410,
            /// <summary>
            /// 波兰语波兰
            /// </summary>
            plPL = 0x0415,
            /// <summary>
            /// 葡萄牙语巴西
            /// </summary>
            ptBR = 0x0416,
            /// <summary>
            /// 俄语俄罗斯
            /// </summary>
            ruRU = 0x0419,
            /// <summary>
            /// 韩语韩国
            /// </summary>
            koKR = 0x0412,
        }
        /// <summary>
        /// 游戏服务器
        /// </summary>
        public enum EnumGameRegion
        {
            NorthAmerica = 1,
            EuropeAndRussia = 2,
            KoreaAndTaiWan = 3,
            Oceania = 4,
            China = 5,
            SoutheastAsia = 6,
        }

        /// <summary>
        /// HTTP下载参数
        /// </summary>
        private class HttpDownloadParams
        {
            /// <summary>
            /// 进度条窗口
            /// </summary>
            public WH_ProgressWindow Window { set; get; }

            /// <summary>
            /// 文件信息
            /// </summary>
            public FileInfo File { set; get; }

            /// <summary>
            /// 下载文件大小
            /// </summary>
            public long Size { set; get; }
        }

        #endregion

        #region 常量
        #region Table名字
        public const string TableName_GameRegionConfig = "DataTable_GameRegionConfig";
        public const string TableName_DataTable_CacheFileConfig = "DataTable_CacheFileConfig";
        public const string TableName_LanguageConfig = "DataTable_LanguageConfig";
        #endregion
        #region 列名 
        public const string ColumnName_GameRegion_RegionIndex = "RegionIndex";
        public const string ColumnName_GameRegion_StartGameLink = "StartGameLink";
        public const string ColumnName_GameRegion_FilePath = "FilePath";
        public const string ColumnName_LanguageConifg_ID = "ID";
        public const string ColumnName_LanguageConifg_RegionIndex = "RegionIndex";
        public const string ColumnName_LanguageConifg_LanguageId = "LanguageId";
        public const string ColumnName_LanguageConifg_GameName = "GameName";
        public const string ColumnName_LanguageConifg_ChangeLog = "ChangeLog";
        public const string ColumnName_CacheFile_Index = "Index";
        public const string ColumnName_CacheFile_RegionIndex = "RegionIndex";
        public const string ColumnName_CacheFile_URL = "URL";
        public const string ColumnName_CacheFile_Path = "Path";
        public const string ColumnName_CacheFile_SHA = "SHA";
        public const string ColumnName_CacheFile_FileSize = "FileSize";
        #endregion
        #region 关系
        public const string RelationName_CacheFileRegionIndex = "DataRelation_CacheFileRegionIndex";
        public const string RelationName_ChangeLogRegionIndex = "DataRelation_ChangeLogRegionIndex";
        #endregion
        #endregion

        #region 构造函数
        #endregion

        #region 方法

        /// <summary>
        /// 添加游戏区域
        /// </summary>
        /// <param name="region">区域</param>
        /// <param name="startGameLink">启动游戏链接</param>
        /// <param name="filePath">录像文件路径</param>
        public void AddNewRegion(EnumGameRegion region, string startGameLink, string filePath)
        {
            Tables[TableName_GameRegionConfig].Rows.Add(region, startGameLink, filePath);
        }

        /// <summary>
        /// 添加语言信息
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="language">使用语言</param>
        /// <param name="region">所属区域</param>
        /// <param name="changeLog">更新日志</param>
        public void AddNewLanguage(int id, EnumLanguage language, EnumGameRegion region, string name, string changeLog)
        {
            DataRow regionRow = Tables[TableName_GameRegionConfig].Rows.Find((int)region);
            Tables[TableName_LanguageConfig].Rows.Add(id, regionRow[ColumnName_GameRegion_RegionIndex], language, name, changeLog);
        }

        /// <summary>
        /// 下载缓存文件
        /// </summary>
        /// <param name="currentLanguage">当前语言</param>
        /// <param name="file">下载目标文件</param>
        /// <param name="url">在线URL</param>
        /// <param name="fileSize">初始文件大小</param>
        /// <returns>下载结果</returns>
        private bool DownloadCacheFileStart(ResourceDictionary currentLanguage, FileInfo file, string url, long fileSize)
        {
            return WH_CommonFunction.HttpDownloadTaskWithWindow(url, file.FullName, 0, null, null, currentLanguage["UI_ProgressWindow_Title"] as string, file.Name, currentLanguage["UI_ProgressWindow_DownloadSize"] as string, currentLanguage["UI_ProgressWindow_StopButton"] as string);
        }

        /// <summary>
        /// 添加缓存文件
        /// </summary>
        /// <param name="region">所属区域</param>
        /// <param name="url">下载Url</param>
        /// <returns>添加结果</returns>
        public bool AddNewCacheFile(EnumGameRegion region, string url, ResourceDictionary currentLanguage)
        {
            string name = url.Substring(url.LastIndexOf("/") + 1);
            string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Blizzard Entertainment\\Battle.net\\Cache\\" + name.Substring(0, 2) + "\\" + name.Substring(2, 2) + "\\" + name;
            FileInfo file = new FileInfo(path);
            long fileSize = WH_CommonFunction.HttpGetLength(url);
            if (file.Exists)
            {
                if (fileSize != file.Length)
                {
                    file.Delete();
                    if (!DownloadCacheFileStart(currentLanguage, file, url, fileSize))
                    {
                        return false;
                    }
                }
            }
            else
            {
                if (!DownloadCacheFileStart(currentLanguage, file, url, fileSize))
                {
                    return false;
                }
            }
            FileStream fs = new FileStream(file.FullName, FileMode.Open);
            SHA1 sha = new SHA1CryptoServiceProvider();
            Byte[] shaBytes = sha.ComputeHash(fs);
            DataRow regionRow = Tables[TableName_GameRegionConfig].Rows.Find((int)region);
            Tables[TableName_DataTable_CacheFileConfig].Rows.Add(Tables[TableName_DataTable_CacheFileConfig].Rows.Count, regionRow[ColumnName_GameRegion_RegionIndex], url, path.Replace(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ""), shaBytes, file.Length);
            fs.Close();
            return true;
        }

        /// <summary>
        /// 获取游戏区域的缓存文件数据
        /// </summary>
        /// <param name="region">游戏区域</param>
        /// <returns>缓存文件数据DataRow</returns>
        public DataRow[] GetCacheFileInfoForRegion(EnumGameRegion region)
        {
            DataRow regionRow = Tables[TableName_GameRegionConfig].Rows.Find((int)region);
            return regionRow.GetChildRows(RelationName_CacheFileRegionIndex);
        }

        /// <summary>
        /// 获取游戏区域的更新日志文件
        /// </summary>
        /// <param name="region">游戏区域</param>
        /// <returns>更新日志DataRow</returns>
        public DataRow[] GetChangeLogForRegion(EnumGameRegion region)
        {
            DataRow regionRow = Tables[TableName_GameRegionConfig].Rows.Find((int)region);
            return regionRow.GetChildRows(RelationName_ChangeLogRegionIndex);
        }

        /// <summary>      
        /// 序列化DataSet对象并压缩      
        /// </summary>      
        /// <param name="savePath">保存路径</param>    
        public void DataSetSerializerCompression(FileInfo savePath)
        {
            IFormatter formatter = new BinaryFormatter();//定义BinaryFormatter以序列化DataSet对象   
            MemoryStream ms = new MemoryStream();//创建内存流对象   
            formatter.Serialize(ms, this);//把DataSet对象序列化到内存流   
            byte[] buffer = ms.ToArray();//把内存流对象写入字节数组   
            ms.Close();//关闭内存流对象   
            ms.Dispose();//释放资源   
            if (!savePath.Directory.Exists)
            {
                savePath.Directory.Create();
            }

            FileStream fs = savePath.Create();//创建文件   
            GZipStream gzipStream = new GZipStream(fs, CompressionMode.Compress, true);//创建压缩对象   
            gzipStream.Write(buffer, 0, buffer.Length);//把压缩后的数据写入文件   
            gzipStream.Close();//关闭压缩流   
            gzipStream.Dispose();//释放对象   
            fs.Close();//关闭流   
            fs.Dispose();//释放对象   
        }

        /// <summary>   
        /// 反序列化压缩的DataSet   
        /// </summary>   
        /// <param name="filePath">读取路径</param>   
        /// <returns></returns>   
        public static SC2_MapInfoDataSet DataSetDeserializeDecompress(FileInfo filePath)
        {
            FileStream fs = filePath.OpenRead();//打开文件   
            fs.Position = 0;//设置文件流的位置   
            GZipStream gzipStream = new GZipStream(fs, CompressionMode.Decompress);//创建解压对象   
            byte[] buffer = new byte[4096];//定义数据缓冲   
            int offset = 0;//定义读取位置   
            MemoryStream ms = new MemoryStream();//定义内存流   
            while ((offset = gzipStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, offset);//解压后的数据写入内存流   
            }
            BinaryFormatter sfFormatter = new BinaryFormatter();//定义BinaryFormatter以反序列化DataSet对象   
            ms.Position = 0;//设置内存流的位置   
            SC2_MapInfoDataSet ds;
            try
            {
                ds = (SC2_MapInfoDataSet)sfFormatter.Deserialize(ms);//反序列化   
            }
            catch
            {
                throw;
            }
            finally
            {
                ms.Close();//关闭内存流   
                ms.Dispose();//释放资源   
            }
            fs.Close();//关闭文件流   
            fs.Dispose();//释放资源   
            gzipStream.Close();//关闭解压缩流   
            gzipStream.Dispose();//释放资源   
            return ds;
        }

        /// <summary>   
        /// 反序列化压缩的DataSet   
        /// </summary>   
        /// <param name="filePath">读取路径</param>   
        /// <returns></returns>   
        public static SC2_MapInfoDataSet DataSetDeserializeDecompress(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            Stream responseStream = response.GetResponseStream();
            GZipStream gzipStream = new GZipStream(responseStream, CompressionMode.Decompress);//创建解压对象   
            byte[] buffer = new byte[4096];//定义数据缓冲   
            int offset = 0;//定义读取位置   
            MemoryStream ms = new MemoryStream();//定义内存流   
            while ((offset = gzipStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                ms.Write(buffer, 0, offset);//解压后的数据写入内存流   
            }
            BinaryFormatter sfFormatter = new BinaryFormatter
            {
                Binder = new UBinder()
            };//定义BinaryFormatter以反序列化DataSet对象   
            ms.Position = 0;//设置内存流的位置   
            SC2_MapInfoDataSet ds;
            try
            {
                ds = (SC2_MapInfoDataSet)sfFormatter.Deserialize(ms);//反序列化   
            }
            catch
            {
                throw;
            }
            finally
            {
                ms.Close();//关闭内存流   
                ms.Dispose();//释放资源   
            }
            responseStream.Close();//关闭文件流   
            responseStream.Dispose();//释放资源   
            gzipStream.Close();//关闭解压缩流   
            gzipStream.Dispose();//释放资源   
            response.Close();
            return ds;
        }
        #endregion
    }
}
