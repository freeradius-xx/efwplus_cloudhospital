using ProtoBuf;
using System;
using System.Xml.Serialization;

namespace EFWCoreLib.CoreFrame.Business
{
    /// <summary>
    /// 系统登录后存在Session中用户的信息
    /// </summary>
    [Serializable]
    [ProtoContract]
    public class SysLoginRight
    {
        private int _userId;
        [XmlElement]
        [ProtoMember(1)]
        public int UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }
        private int _empId;
        [XmlElement]
        [ProtoMember(2)]
        public int EmpId
        {
            get { return _empId; }
            set { _empId = value; }
        }
        private string _empName;
        [XmlElement]
        [ProtoMember(3)]
        public string EmpName
        {
            get { return _empName; }
            set { _empName = value; }
        }
        private int _deptId;
        [XmlElement]
        [ProtoMember(4)]
        public int DeptId
        {
            get { return _deptId; }
            set { _deptId = value; }
        }
        private string _deptName;
        /// <summary>
        /// 当前登录科室
        /// </summary>
        [XmlElement]
        [ProtoMember(5)]
        public string DeptName
        {
            get { return _deptName; }
            set { _deptName = value; }
        }
        private int _workId;
        [XmlElement]
        [ProtoMember(6)]
        public int WorkId
        {
            get { return _workId; }
            set { _workId = value; }
        }

        private string _workName;
        [XmlElement]
        [ProtoMember(7)]
        public string WorkName
        {
            get { return _workName; }
            set { _workName = value; }
        }

        [XmlElement]
        [ProtoMember(8)]
        public Guid token { get; set; }
    }
}
