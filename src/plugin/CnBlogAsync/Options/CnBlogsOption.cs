namespace CnBlogAsync.Options
{
    /// <summary>
    /// 博客园配置
    /// </summary>
    public class CnBlogsOption : ClientOption
    {
        public CnBlogsOption(string blogname, string username, string password)
        {
            MetaWeblogURL = "https://rpc.cnblogs.com/metaweblog/" + blogname;
            BlogURL = "https://www.cnblogs.com";
            Username = username;
            Password = password;
        }
    }
}
