using System.Xml.Serialization;

namespace CnBlogAsync.Models;

public class PostInfo
{
    public List<string> Categories = new();

    public string Title { get; set; }

    public string Link { get; set; }

    public DateTime? DateCreated { get; set; }

    public string PostID { get; set; }

    public string UserID { get; set; }

    public int CommentCount { get; set; }

    public string PostStatus { get; set; }

    public string PermaLink { get; set; }

    public string Description { get; set; }

    public static void Serialize(PostInfo[] posts, string filename)
    {
        var serializer = new XmlSerializer(typeof(PostInfo[]));
        var textWriter = new StreamWriter(filename);
        serializer.Serialize(textWriter, posts);
        textWriter.Close();
    }

    public static PostInfo[] Deserialize(string filename)
    {
        var fp = File.OpenText(filename);
        var posts_serializer = new XmlSerializer(typeof(PostInfo[]));
        var loaded_posts = (PostInfo[])posts_serializer.Deserialize(fp);
        fp.Close();
        return loaded_posts;
    }
}