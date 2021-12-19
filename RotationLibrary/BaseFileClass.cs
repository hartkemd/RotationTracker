using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationLibrary
{
    public abstract class BaseFileClass
    {
        /// <summary>
        /// Directory where file containing this object's data lives.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File where this object's data lives.
        /// </summary>
        public string FileName { get; set; }

        public string FullFilePath => $"{FilePath}{FileName}";
    }
}
