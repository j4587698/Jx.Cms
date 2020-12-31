﻿using System.ComponentModel;
using FreeSql;

namespace Jx.Cms.Entities.Article
{
    [Description("标签")]
    public class LabelEntity:BaseEntity<LabelEntity, int>
    {
        [Description("标签名")]
        public string Name { get; set; }
        
    }
}