using System;
using System.Collections.Generic;
using System.Text;

namespace SimplCommerce.Infrastructure
{
    /// <summary>
    /// 用於定義版本的釋出階段, 區分為開發、測試、釋出
    /// Develop,Test,Release
    /// </summary>
    public enum VersionStageType
    {
        /// <summary>
        /// 開發階段
        /// </summary>
        Develop,

        /// <summary>
        /// 測試階段
        /// </summary>
        Test,

        /// <summary>
        /// 釋出階段
        /// </summary>
        Release
    }
}
