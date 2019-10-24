﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using vc_webapi.Model;
using vc_webapi.Data;
using vc_webapi.Helpers;
using Microsoft.AspNetCore.Authorization;
using vc_webapi.Model.Users;
using vc_webapi.Model.API;
using Microsoft.EntityFrameworkCore.Query;

namespace vc_webapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly Vc_webapiContext db;

        public CommentsController(Vc_webapiContext context)
        {
            db = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] Comment comment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            User user = await this.User(db);

            var video = await db.Videos.FindAsync(comment.Video);

            if (user != null)
            {
                db.Comments.Add(new Comment
                {
                    UserName = user.UserName,
                    CommentTime = DateTime.Now,
                    Message = comment.Message,
                    Video = video

                }) ;
                await db.SaveChangesAsync();
                return Ok();
            }
            return Unauthorized();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetComments(long id)
        {
            var comments = await db.Comments.FindAsync(id);

            if (comments == null)
            {
                return NotFound();
            }
            return Ok(comments);
        }
    }
}

    

       