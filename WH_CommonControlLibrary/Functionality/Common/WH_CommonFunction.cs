using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WH_CommonControlLibrary.UIControl.UIWindow;
using static WH_CommonControlLibrary.UIControl.UIWindow.WH_ProgressWindow;

namespace WH_CommonControlLibrary.Functionality.Common
{
    /// <summary>
    /// 常用函数
    /// </summary>
    public static class WH_CommonFunction
    {
        #region 声明

        /// <summary>
        /// 下载过程回调委托
        /// </summary>
        /// <param name="downSize">下载大小</param>
        /// <param name="inParams">下次回调参数</param>
        public delegate bool Delegate_HttpDownLoadProgreess(long downSize, object inParams);

        /// <summary>
        /// Http下载参数
        /// </summary>
        public class WH_HttpDownloadParams
        {
            #region 属性

            /// <summary>
            /// 下载地址
            /// </summary>
            public string URL { private set; get; }

            /// <summary>
            /// 文件路径
            /// </summary>
            public string Path { private set; get; }

            /// <summary>
            /// 超时时间
            /// </summary>
            public int TimeOut { private set; get; }

            /// <summary>
            /// 回调方法
            /// </summary>
            public Delegate_HttpDownLoadProgreess ProgressCallBackFunc { private set; get; }

            /// <summary>
            /// 回调参数
            /// </summary>
            public object ProgressCallBacParams { private set; get; }

            /// <summary>
            /// Title文本
            /// </summary>
            public string Title { set; get; }

            /// <summary>
            /// 主要文本
            /// </summary>
            public string Primary { set; get; }

            /// <summary>
            /// 次要文本
            /// </summary>
            public string Secondary { set; get; }

            /// <summary>
            /// 停止文本
            /// </summary>
            public string Stop { set; get; }

            /// <summary>
            /// 停止回调
            /// </summary>
            public Delegate_ProgressWindowStop StopCallBackFunc { set; get; }

            /// <summary>
            /// 停止回调参数
            /// </summary>
            public object StopCallBackParams { set; get; }

            /// <summary>
            /// 下载文件大小
            /// </summary>
            public long FileSize { set; get; }

            /// <summary>
            /// 进度条窗口
            /// </summary>
            public WH_ProgressWindow Window { private set; get; }            

            #endregion

            #region 构造函数

            /// <summary>
            /// 
            /// </summary>
            /// <param name="url">下载文件地址</param>
            /// <param name="path">文件存放地址，包含文件名</param>
            /// <param name="timeOut">下载超时时间，负数或0为默认值</param>
            /// <param name="inParams">参数</param>
            /// <param name="callback">下载进程回调</param>
            public WH_HttpDownloadParams(string url, string path, int timeOut, object inParams, Delegate_HttpDownLoadProgreess callback)
            {
                URL = url;
                Path = path;
                TimeOut = timeOut;
                ProgressCallBacParams = inParams;
                ProgressCallBackFunc = callback;
            }

            #endregion

            #region 方法

            /// <summary>
            /// 新建进度窗口
            /// </summary>
            /// <returns>进度条窗口</returns>
            public WH_ProgressWindow NewWindow()
            {
                if (Window == null) Window = new WH_ProgressWindow(Title, Primary, Secondary, "0K/" + (FileSize / 1024) + "K", Stop, FileSize, StopCallBackFunc, StopCallBackParams);
                return Window;
            }

            #endregion
        }
        #endregion

        #region 属性

        #endregion

        #region 方法

        #region HTTP下载

        /// <summary>
        /// Http协议获取文件长度
        /// </summary>
        /// <param name="url">文件URL</param>
        /// <returns>下载长度</returns>
        public static long HttpGetLength(string url)
        {
            long length = 0;
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                req.Method = "HEAD";
                req.Timeout = 5000;
                var res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    length = res.ContentLength;
                }

                res.Close();
                return length;
            }
            catch (Exception err)
            {
                return 0;
            }
        }

        /// <summary>
        /// Http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <param name="timeOut">下载超时时间，负数或0为默认值</param>
        /// <param name="inParams">参数</param>
        /// <param name="callback">下载进程回调</param>
        /// <returns>下载成功</returns>
        public static bool HttpDownload(string url, string path, int timeOut, object inParams, Delegate_HttpDownLoadProgreess callback)
        {
            #region 准备下载路径

            string tempPath = System.IO.Path.GetDirectoryName(path) + @"\temp";
            DirectoryInfo tempDir = new DirectoryInfo(tempPath);
            if (!tempDir.Exists) tempDir.Create();
            string tempFile = tempPath + @"\" + System.IO.Path.GetFileName(path) + ".temp"; //临时文件
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);    //存在则删除
            }
            FileStream fs = new FileStream(tempFile, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);

            #endregion

            #region 下载过程

            // 设置参数
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Proxy = null;
            if (timeOut > 0) request.Timeout = timeOut;
            //发送请求并获取相应回应数据
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            //直到request.GetResponse()程序才开始向目标网页发送Post请求
            Stream responseStream = response.GetResponseStream();
            byte[] bArr = new byte[1024];
            int size = responseStream.Read(bArr, 0, (int)bArr.Length);
            int downSize = size;
            bool isStop = false;
            while (size > 0)
            {
                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    if (callback != null) isStop = callback(downSize, inParams);
                }));

                if (isStop) break;
                fs.Write(bArr, 0, size);
                size = responseStream.Read(bArr, 0, (int)bArr.Length);
                downSize += size;
            }

            #endregion

            #region 下载后清理
            fs.Close();
            responseStream.Close();
            if (isStop)
            {
                File.Delete(tempFile);
            }
            else
            {
                System.IO.File.Move(tempFile, path);
            }
            tempDir.Delete();
            return !isStop;
            #endregion
        }

        /// <summary>
        /// Http下载Task任务函数有返回
        /// </summary>
        /// <param name="inParams">参数</param>
        /// <returns>任务结果</returns>
        private static bool HttpDownloadTaskFunc(object inParams)
        {
            WH_HttpDownloadParams param = inParams as WH_HttpDownloadParams;
            return HttpDownload(param.URL, param.Path, param.TimeOut, param.ProgressCallBacParams, param.ProgressCallBackFunc);
        }

        /// <summary>
        /// 新任务Http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <param name="timeOut">下载超时时间，负数或0为默认值</param>
        /// <param name="inParams">参数</param>
        /// <param name="callback">下载进程回调</param>
        /// <returns>下载成功</returns>
        public static bool HttpDownloadTask(string url, string path, int timeOut, object inParams, Delegate_HttpDownLoadProgreess callback)
        {
            WH_HttpDownloadParams param = new WH_HttpDownloadParams(url, path, timeOut, inParams, callback);
            Task<bool> task = new Task<bool>(HttpDownloadTaskFunc, param);
            task.Start();
            return task.Result;
        }

        /// <summary>
        /// Http下载Task任务过程回调
        /// </summary>
        /// <param name="downSize"></param>
        /// <param name="inParams"></param>
        private static bool HttpDownloadTaskWithWindowProgressCallBack(long downSize, object inParams)
        {
            WH_HttpDownloadParams param = inParams as WH_HttpDownloadParams;
            // 执行真正的进程回调
            param.ProgressCallBackFunc?.Invoke(downSize, param.ProgressCallBacParams);
            param.Window.SetProgressValue(downSize);
            param.Window.SetSecondaryRightText((downSize / 1024) + "K/" + (param.FileSize / 1024) + "K");
            return param.Window.IsStop;
        }

        /// <summary>
        /// Http下载Task任务
        /// </summary>
        /// <param name="inParams">参数</param>
        /// <returns>任务结果</returns>
        private static void HttpDownloadTaskWithWindowFunc(object inParams)
        {
            WH_HttpDownloadParams param = inParams as WH_HttpDownloadParams;
            try
            {
                HttpDownload(param.URL, param.Path, param.TimeOut, param, HttpDownloadTaskWithWindowProgressCallBack);
            }
            catch
            {
                param.Window.IsStop = true;
            }
        }

        /// <summary>
        /// 新任务Http下载文件
        /// </summary>
        /// <param name="url">下载文件地址</param>
        /// <param name="path">文件存放地址，包含文件名</param>
        /// <param name="timeOut">下载超时时间，负数或0为默认值</param>
        /// <param name="inParams">参数</param>
        /// <param name="title">Title文本</param>
        /// <param name="primary">主要文本</param>
        /// <param name="secondary">次要</param>
        /// <param name="stop">停止文本</param>
        /// <returns>下载成功</returns>
        public static bool HttpDownloadTaskWithWindow(string url, string path, int timeOut, object inParams, Delegate_HttpDownLoadProgreess callback, string title, string primary, string secondary, string stop)
        {
            WH_HttpDownloadParams param = new WH_HttpDownloadParams(url, path, timeOut, inParams, callback);
            param.FileSize = HttpGetLength(param.URL);
            param.Title = title;
            param.Primary = primary;
            param.Secondary = secondary;
            param.Stop = stop;
            param.StopCallBackFunc = null;
            param.StopCallBackParams = null;
            param.NewWindow();
            Task task = new Task(HttpDownloadTaskWithWindowFunc, param);
            task.Start();
            param.Window.ShowDialog();
            return !param.Window.IsStop;
        }
        #endregion

        #endregion
    }
}
