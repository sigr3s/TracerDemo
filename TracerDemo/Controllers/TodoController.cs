﻿
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TracerDemo.Data;
using Microsoft.AspNetCore.Authorization;
using TracerDemo.Model;
using MongoDB.Driver;
using TracerDemo.Helpers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TracerDemo.Controllers
{
    public class TodoController : Controller
    {
        private SqliteContext db { get; set; }
        private UserHelper UserHelper { get; set; }
        public TodoController(SqliteContext sqliteContext, UserHelper userHelper)
        {
            db = sqliteContext;
            UserHelper = userHelper;
        }


        [HttpGet]
        [Route("api/v1/users/{id}/todo")]
        [Authorize]
        public IActionResult GetUserTodoItems(string id)
        {
            User user = UserHelper.GetUserById(User.Identity.Name);

            //You must be logged in as the user specified in the URL
            if (id != user?.Id)
                return BadRequest("Invalid Permissions");

            List<Todo> list = db.Todos.Where(t => t.Owner == user.Id).ToList();

            return Ok(list);
        }

        [HttpPost]
        [Authorize]
        [Route("api/v1/users/{id}/todo")]
        public IActionResult CreateItem(string id, [FromBody]TodoItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = UserHelper.GetUserById(User.Identity.Name);

                //You must be logged in as the user specified in the URL
                if (id != user?.Id)
                    return BadRequest("Invalid Permissions");

                if (model.Task == null || model.Task.Trim() == "")
                  return BadRequest();

                Todo item = new Todo()
                {
                    Owner = user.Id,
                    Completed = false,
                    Task = model.Task
                };

                db.Todos.Add(item);
                return Ok(item);
            }
            else
                return BadRequest();
        }

        [HttpPut]
        [Authorize]
        [Route("api/v1/users/{userId}/todo/{itemId}/status/{completed}")]
        public IActionResult CompleteItem(string userId, string itemId, bool completed)
        {
            if (ModelState.IsValid)
            {
                User user = UserHelper.GetUserById(User.Identity.Name);

                //You must be logged in as the user specified in the URL
                if (userId != user?.Id)
                    return BadRequest("Invalid Permissions");

                Todo item = db.Todos.Where(t => t.Owner == userId && t.Id == itemId).FirstOrDefault();

                if (item == null)
                    return NotFound();

                item.Completed = true;
                db.Todos.Update(item);

                return Ok();
            }
            else
                return BadRequest();
        }

        [HttpDelete]
        [Authorize]
        [Route("api/v1/users/{userId}/todo/{itemId}")]
        public IActionResult DeleteItem(string userId, string itemId)
        {
            if (ModelState.IsValid)
            {
                User user = UserHelper.GetUserById(User.Identity.Name);

                //You must be logged in as the user specified in the URL
                if (userId != user?.Id)
                    return BadRequest("Invalid Permissions");

                Todo item = db.Todos.Where(t => t.Id == itemId && t.Owner == userId).FirstOrDefault();
                if (item == null)
                    return NotFound();

                db.Todos.Update(item);
                return Ok();
            }
            else
                return BadRequest();
        }

    }
}
