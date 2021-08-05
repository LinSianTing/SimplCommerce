using System;
using System.Collections.Generic;
using System.Text;
using SimplCommerce.Infrastructure;

namespace SimplCommerce.Module.Core.Models
{
    /// <summary>
    /// Add By Eric.Lin
    /// Usage : for System 多站台設計
    /// Date : 20200922
    /// </summary>
    public class SystemApp
    {
        public SystemApp(int id, string name, string description, bool enableSystemIdCross, List<int> IdCross,VersionStageType versionStage)
        {
            m_Id = id;
            m_Name = name;
            m_Description = description;
            m_EnableSystemIdCross = enableSystemIdCross;
            m_IdCrossList = IdCross;
            m_VersionStage = versionStage;
        }

        private int m_Id { get; set; }
        public int Id { get { return m_Id; }}

        private string m_Name { get; set; }
        public string Name { get { return m_Name; } }

        private string m_Description { get; set; }
        public string Description { get { return m_Description; } }

        private bool m_EnableSystemIdCross { get; set; }
        public bool EnableSystemIdCross { get { return m_EnableSystemIdCross; } }

        private List<int> m_IdCrossList { get; set; }
        public List<int> IdCrossList { get { return m_IdCrossList; } }

        private VersionStageType m_VersionStage { get; set; }
        public VersionStageType VersionStage { get { return m_VersionStage; } }
    }
}
