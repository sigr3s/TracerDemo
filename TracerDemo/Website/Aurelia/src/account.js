import { inject } from 'aurelia-framework';
import { AppService } from './app-service';
import { DialogService } from 'aurelia-dialog';
import { message } from './message';
import { WebAPI } from './web-api';

@inject(WebAPI, AppService, DialogService)
export class Account {
	constructor(WebAPI, AppService, DialogService) {
	  this.api = WebAPI;
	  this.appService = AppService;
	  this.dialogService = DialogService

	  this.getCurrentUser();
  }

  getCurrentUser()
  {
	  //To ensure the account page is populated with all the latest user data, call the same service used to populate the user session.
    //Might as well make sure the session has all the latest info.
	  this.api.getCurrentUser().then(r => {
		  if (r.success === true) {
			  this.appService.user = r.content;
			  localStorage.setItem('authenticated', true);
		  }
		  else {
			  localStorage.removeItem('authenticated');
			  this.appService.user = null;
		  }
	  });
  }

  saveUserChanges()
  {
	  this.api.updateUserById(this.appService.user.id, this.appService.user.email, this.appService.user.name).then(r => {
		  if (r.success === true) {
			  this.dialogService.open({ viewModel: message, model: { message: 'Account Updated', confirm: 'Ok' }, lock: false });
		  }
		  else {
			  this.dialogService.open({ viewModel: message, model: { message: r.content, confirm: 'Ok' }, lock: false });
			  this.message = r.content;
		  }
	  });
  }

  deleteUser()
  {
	  this.dialogService.open({ viewModel: message, model: { message: 'Are you sure you want to delete this user?', confirm: 'Yes. Do it.', cancel: 'No!'}, lock: false }).whenClosed(response => {
		  if (!response.wasCancelled) {
			  //success/yes
			  this.api.deleteUserById(this.appService.user.id).then(r => {
				  if (r.success === true) {
					  localStorage.removeItem('authenticated');
					  this.appService.user = null;
					  location.href = "/signin";//after signing out, refresh the whole application. This will ensure the sessions is totally cleaned out.
				  }
				  else {
            this.dialogService.open({ viewModel: message, model: { message: r.content, confirm: 'Ok' }, lock: false });
				  }
			  });
		  } else {
			  //failure/no
		  }
	  });

	  
  }

  changeUserPassword()
  {
	  this.api.changeUserPasswordById(this.appService.user.id, this.currentPassword, this.newPassword, this.confirmPassword).then(r => {
		  if (r.success === true) {
			  this.dialogService.open({ viewModel: message, model: { message: 'Password Changed', confirm: 'Ok' }, lock: false });
		  }
		  else {
			  this.dialogService.open({ viewModel: message, model: { message: r.content, confirm: 'Ok' }, lock: false });
		  }
	  });
  }
}
