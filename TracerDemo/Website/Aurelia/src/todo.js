import { inject } from 'aurelia-framework';
import { AppService } from './app-service';
import { WebAPI } from './web-api';
import { DialogService } from 'aurelia-dialog';
import { message } from './message';

@inject(WebAPI, AppService, DialogService)
export class Todo {     
  constructor(WebAPI, AppService, DialogService) {
    this.api = WebAPI;
	  this.appService = AppService;
    this.dialogService = DialogService

	  this.todos = [];

    //Do this for start up processes that are dependent upon user.id being an actual value
	  if (this.appService.user != null)
	    this.getTodos();
  }

  createTodo()
  {
		this.api.createTodoByUser(this.appService.user.id, this.newTask)
		  .then(r => {
			  if (r.success === true) {
				  this.newTask = undefined;
				  this.todos.push(r.content);
			  }
			  else {
				  this.results = r.content;
				  this.error = r.success === false;
			  }
		  });
  }

  getTodos()
  {
	  this.api.getTodosByUserId(this.appService.user.id)
		  .then(r => {
			  if (r.success === true) {
				  this.todos = r.content;
			  }
			  else {
				  this.results = r.content;
				  this.error = r.success === false;
			  }
		  });
  }

  deleteTodo(item)
  {
	  this.dialogService.open({ viewModel: message, model: { message:'Are you sure you want to delete this item?', confirm: 'Yes. Do it.', cancel: 'No!' }, lock: false }).whenClosed(response => {
		  if (!response.wasCancelled)
		  {
        //success/yes
			  this.api.deleteTodoById(this.appService.user.id, item.id)
				  .then(r => {
					  if (r.success === true) {
						  this.todos = this.todos.filter(t => t.id !== item.id);
					  }
					  else {
						  this.results = r.content;
						  this.error = r.success === false;
					  }
				  });
		  } else
		  {
        //failure/no
			}
		});




  }

  completeTodo(item) {
	  item.completed = item.completed ? false : true;
	  this.api.completeTodoById(this.appService.user.id, item.id, item.completed)
		  .then(r => {
			  if (r.success === true) {
			  }
			  else {
				  this.results = r.content;
				  this.error = r.success === false;
			  }
		  });
  }
  
}
