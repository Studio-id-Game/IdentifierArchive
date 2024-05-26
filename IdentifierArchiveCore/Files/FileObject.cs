using System.Runtime.Serialization;
using System.Text;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public abstract class FileObject<TController, TSelf>
        where TSelf : FileObject<TController, TSelf>
        where TController : FileController<TSelf, TController>, new()
    {
        public static FileController<TSelf> Controller { get; } = FileController<TSelf, TController>.Instance;

        protected FileObject()
        {
            Self = (TSelf)this;
        }

        [IgnoreDataMember]
        protected TSelf Self { get; }

        [IgnoreDataMember]
        public abstract string ScreenName { get; }

        [IgnoreDataMember]
        public abstract string FileName { get; }

        public FileInfo GetFileInfo(DirectoryInfo directoryInfo)
        {
            return new($"{directoryInfo.FullName}/{FileName}");
        }

        public TSelf? FromFile(DirectoryInfo directoryInfo)
        {
            var data = Controller.FromFile(GetFileInfo(directoryInfo), ScreenName);
            
            if(data == null) return null;
            
            Self.CopyFrom(data);
            return Self;
        }

        public bool ToFile(DirectoryInfo directoryInfo, out bool created, out bool overwrited, bool? autoCreate = false, bool? autoOverwrite = false, bool backupToRecycleBin = true)
        {
            return Controller.ToFile(Self, GetFileInfo(directoryInfo), ScreenName, out created, out overwrited, autoCreate, autoOverwrite, backupToRecycleBin);
        }

        public bool CreateOrUpdate(DirectoryInfo directoryInfo)
        {
            var exists = ToFile(directoryInfo, out bool created, out bool overwrited, autoCreate: true, autoOverwrite: true, backupToRecycleBin: false);
            if(exists) 
            {
                return overwrited;
            }
            else
            {
                return created;
            }
        }

        public abstract void CopyFrom(TSelf other);

        public override string ToString()
        {
            return $"{ScreenName} ({FileName})";
        }

        public abstract string AsText();
    }
}