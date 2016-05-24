using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace EFWCoreLib.WcfFrame.WcfService.Contract
{
    /// <summary>
    /// 文件传输服务
    /// </summary>
    [ServiceKnownType(typeof(DBNull))]
    [ServiceContract(Namespace = "http://www.efwplus.cn/", Name = "FileTransferHandlerService", SessionMode = SessionMode.Allowed)]
    public interface IFileTransfer
    {
        //上传文件
        [OperationContract]
        UpFileResult UpLoadFile(UpFile filestream);

        //上传进度
        [OperationContract]
        int GetUpLoadFileProgress(string upkey);

        //下载文件
        [OperationContract]
        DownFileResult DownLoadFile(DownFile downfile);

        //下载进度
        [OperationContract]
        void SetDownLoadFileProgress(string clientId, string downkey, int progressnum);
    }

    [MessageContract]
    public class DownFile
    {
        [MessageHeader]
        public string clientId { get; set; }
        [MessageHeader]
        public string DownKey { get; set; }
        [MessageHeader]
        public string FileName { get; set; }
    }

    [MessageContract]
    public class UpFileResult
    {
        [MessageHeader]
        public bool IsSuccess { get; set; }
        [MessageHeader]
        public string Message { get; set; }
    }

    [MessageContract]
    public class UpFile
    {
        [MessageHeader]
        public string clientId { get; set; }
        [MessageHeader]
        public string UpKey { get; set; }
        [MessageHeader]
        public long FileSize { get; set; }
        [MessageHeader]
        public string FileName { get; set; }
        [MessageHeader]
        public string FileExt { get; set; }//带.
        [MessageBodyMember]
        public Stream FileStream { get; set; }
    }

    [MessageContract]
    public class DownFileResult
    {
        [MessageHeader]
        public long FileSize { get; set; }
        [MessageHeader]
        public bool IsSuccess { get; set; }
        [MessageHeader]
        public string Message { get; set; }
        [MessageBodyMember]
        public Stream FileStream { get; set; }
    }
}
