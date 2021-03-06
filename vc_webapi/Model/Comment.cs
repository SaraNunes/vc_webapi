﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace vc_webapi.Model
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; internal set; }
        public User User { get; set; }
        [JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public Video Video { get; set; }
        public string Message { get; set; }
        public DateTime CommentTime { get; set; }
        [NotMapped]
        public bool Deletable { get; set; }
    }
}
