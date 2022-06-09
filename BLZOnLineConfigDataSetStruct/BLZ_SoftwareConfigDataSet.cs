using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using EnumLanguage = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumLanguage;
using EnumGameRegion = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumGameRegion;

namespace BLZOnLineConfigDataSetStruct
{
    /// <summary>
    /// 软件配置
    /// </summary>
    partial class BLZ_SoftwareConfigDataSet
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
        #endregion

        #region 常量      

        #region 表名
        public const string TableName_VersionInfo = "DataTable_VersionInfo";
        public const string TableName_LanguageConfig = "DataTable_LanguageConfig";
        #endregion

        #region Row名
        public const string ColumnName_VersionInfo_VersionString = "VersionString";
        public const string ColumnName_VersionInfo_MajorVersion = "MajorVersion";
        public const string ColumnName_VersionInfo_MinorVersion = "MinorVersion";
        public const string ColumnName_VersionInfo_BuildVersion = "BuildVersion";
        public const string ColumnName_VersionInfo_RevisedVersion = "RevisedVersion";
        public const string ColumnName_VersionInfo_ConfigAddress = "ConfigAddress";
        public const string ColumnName_VersionInfo_DownloadAddress = "DownloadAddress";
        public const string ColumnName_LanguageConfig_Id = "Id";
        public const string ColumnName_LanguageConfig_LanguageId = "LanguageId";
        public const string ColumnName_LanguageConfig_SoftwareChangeLog = "SoftwareChangeLog";
        public const string ColumnName_LanguageConfig_VersionString = "VersionString";

        #endregion

        #region 关系名
        public const string RelationName_VersionString_LanguageConfig = "Relation_VersionString_LanguageConfig";
        #endregion

        #endregion

        #region 属性
        #endregion

        #region 构造函数

        #endregion

        #region 方法

        /// <summary>
        /// 添加版本信息
        /// </summary>
        /// <param name="verStr">版本字符串</param>
        /// <param name="major">版本1</param>
        /// <param name="minor">版本2</param>
        /// <param name="revised">版本3</param>
        /// <param name="build">版本4</param>
        /// <param name="downloadAddress">下载地址</param>
        public void AddNewVersionInfo(string verStr, int major, int minor, int build, int revised, string downloadAddress, string configAddress)
        {
            Tables[TableName_VersionInfo].Rows.Add(verStr, major, minor, build, revised, downloadAddress, configAddress);
        }

        /// <summary>
        /// 添加语言信息
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="verStr">版本字符串</param>
        /// <param name="softwareChangeLog">更新日志</param>
        public void AddNewLanguage(EnumLanguage language, string verStr, string softwareChangeLog)
        {
            Tables[TableName_LanguageConfig].Rows.Add(verStr + Enum.GetName(typeof(EnumLanguage), language), (int)language, verStr, softwareChangeLog);
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
        public static BLZ_SoftwareConfigDataSet DataSetDeserializeDecompress(FileInfo filePath)
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
            BLZ_SoftwareConfigDataSet ds;
            try
            {
                ds = (BLZ_SoftwareConfigDataSet)sfFormatter.Deserialize(ms);//反序列化   
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
        public static BLZ_SoftwareConfigDataSet DataSetDeserializeDecompress(string url)
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
            BLZ_SoftwareConfigDataSet ds;
            try
            {
                ds = (BLZ_SoftwareConfigDataSet)sfFormatter.Deserialize(ms);//反序列化   
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
