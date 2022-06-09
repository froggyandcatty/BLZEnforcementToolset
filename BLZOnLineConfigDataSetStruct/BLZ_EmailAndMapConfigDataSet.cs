using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace BLZOnLineConfigDataSetStruct
{

    using EnumLanguage = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumLanguage;
    using EnumGameRegion = SC2OnLineConfigDataSetStruct.SC2_MapInfoDataSet.EnumGameRegion;

    /// <summary>
    /// 邮件及地图配置
    /// </summary>
    partial class BLZ_EmailAndMapConfigDataSet
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

        public const string TableName_MapConfig = "DataTable_MapConfig";
        public const string TableName_EmailLanguageConfig = "DataTable_EmailLanguageConfig";
        public const string TableName_EmailGameRegionConfig = "DataTable_EmailGameRegionConfig";
        public const string TableName_Config = "DataTable_Config";

        #endregion

        #region Row名
        public const string ColumnName_MapConfig_MapName = "MapName";
        public const string ColumnName_MapConfig_ConfigAddress = "ConfigAddress";
        public const string ColumnName_EmailLanguageConfig_LanguageId = "LanguageId";
        public const string ColumnName_EmailLanguageConfig_EmailSubject = "EmailSubject";
        public const string ColumnName_EmailLanguageConfig_EmailBody = "EmailBody";
        public const string ColumnName_EmailLanguageConfig_IsBodyHtml = "IsBodyHtml";
        public const string ColumnName_EmailGameRegion_RegionIndex = "RegionIndex";
        public const string ColumnName_EmailGameRegion_EmailSmtpHost = "EmailSmtpHost";
        public const string ColumnName_EmailGameRegion_EmailSmtpPort = "EmailSmtpPort";
        public const string ColumnName_EmailGameRegion_EmailSmtpUser = "EmailSmtpUser";
        public const string ColumnName_EmailGameRegion_EmailSmtpPassword = "EmailSmtpPassword";
        public const string ColumnName_EmailGameRegion_EmailSmtpFrom = "EmailSmtpFrom";
        public const string ColumnName_EmailGameRegion_GameBankNamePrefix = "GameBankNamePrefix";
        public const string ColumnName_EmailGameRegion_AuthorPlayerHandle = "AuthorPlayerHandle";
        public const string ColumnName_Config_GameCacheConfig = "GameCacheConfig";
        #endregion

        #endregion

        #region 属性

        #endregion

        #region 构造函数

        #endregion

        #region 方法

        /// <summary>
        /// 添加地图信息
        /// </summary>
        /// <param name="mapName">地图备注名</param>
        /// <param name="configName">配置地址</param>
        public void AddNewMap(string mapName, string configAddress)
        {
            Tables[TableName_MapConfig].Rows.Add(mapName, configAddress);
        }
        /// <summary>
        /// 添加游戏区域
        /// </summary>
        /// <param name="region">区域</param>
        /// <param name="startGameLink">启动游戏链接</param>
        public void AddNewRegion(EnumGameRegion region, string host, int port, string user, string password, string from, string prefix, string author)
        {
            Tables[TableName_EmailGameRegionConfig].Rows.Add((int)region, host, port, user, password, from, prefix, author);
        }

        /// <summary>
        /// 添加语言信息
        /// </summary>
        /// <param name="language">语言</param>
        /// <param name="emailSubject">邮件主题</param>
        /// <param name="emailBody">邮件内容</param>
        /// <param name="isBodyHtml">html邮件</param>
        public void AddNewLanguage(EnumLanguage language, string emailSubject, string emailBody, bool isBodyHtml)
        {
            Tables[TableName_EmailLanguageConfig].Rows.Add((int)language, emailSubject, emailBody, isBodyHtml);
        }

        /// <summary>
        /// 游戏缓存配置地址
        /// </summary>
        /// <param name="address"></param>
        public void SetBLZGameCacheConfig(string address)
        {
            Tables[TableName_Config].Rows.Clear();
            Tables[TableName_Config].Rows.Add(address);
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
        public static BLZ_EmailAndMapConfigDataSet DataSetDeserializeDecompress(FileInfo filePath)
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
            BLZ_EmailAndMapConfigDataSet ds;
            try
            {
                ds = (BLZ_EmailAndMapConfigDataSet)sfFormatter.Deserialize(ms);//反序列化   
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
        public static BLZ_EmailAndMapConfigDataSet DataSetDeserializeDecompress(string url)
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
            BLZ_EmailAndMapConfigDataSet ds;
            try
            {
                ds = (BLZ_EmailAndMapConfigDataSet)sfFormatter.Deserialize(ms);//反序列化   
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
