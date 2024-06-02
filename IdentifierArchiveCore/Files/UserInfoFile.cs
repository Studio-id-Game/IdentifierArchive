using System.Runtime.Serialization;

namespace StudioIdGames.IdentifierArchiveCore.Files
{
    public class UserInfoFile : TextFileObject<UserInfoFile>
    {
        public override string ScreenName => "UserInfo";

        public override string FileName => CustomFileName + ".userinfo";

        public string CustomFileName { get; init; } = "";

        [IgnoreDataMember]
        public string UserName
        {
            get => Text.Split('\n')[0];
            set
            {
                var sp = Text.Split('\n');
                sp[0] = value;
                Text = string.Join("\n", sp);
            }
        }

        [IgnoreDataMember]
        public string UserID
        {
            get => Text.Split('\n').ElementAtOrDefault(1) ?? "";
            set
            {
                var sp = Text.Split('\n');
                if (sp.Length > 1)
                {
                    sp[1] = value;
                }
                Text = string.Join("\n", sp);
            }
        }

        public override string Text { get; set; } = "UserName\nUserID";

        public string SetNewUserID()
        {
            var id = Guid.NewGuid().ToString().ToUpper();
            UserID = id;
            return id;
        }
    }
}