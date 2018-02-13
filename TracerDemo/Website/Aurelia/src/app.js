import { inject } from 'aurelia-framework';
import { AppService } from './app-service';
import { WebAPI } from './web-api';
import { Redirect } from 'aurelia-router';

@inject(WebAPI, AppService)
export class App {
	constructor(WebAPI, AppService) {
		this.api = WebAPI;
		this.appService = AppService;


	}

	async activate() {
		//if user is authenticated, this will populate the app with the current user data.
		await this.api.getCurrentUser().then(r => {
			if (r.success === true) {
				this.appService.user = r.content;
				localStorage.setItem('authenticated', true);
			}
			else {
				localStorage.setItem('authenticated', false);
				this.appService.user = null;
			}
		});
	}

	logout() {
		this.api.logoutUser().then(r => {
			if (r.success === true) {
				//Do the same thing if the logout succeeds or fails.
				localStorage.removeItem('authenticated');
				this.appService.user = null;
				location.href = "/signin";//after signing out, refresh the whole application. This will ensure the sessions is totally cleaned out.
			}
			else {
				//Do the same thing if the logout succeeds or fails.
				localStorage.removeItem('authenticated');
				this.appService.user = null;
				location.href = "/signin";//after signing out, refresh the whole application. This will ensure the sessions is totally cleaned out.
			}
		});
	}

	configureRouter(config, router) {
		config.title = 'au.Net.Mongo';
		config.options.pushState = true; //needed to add this to help with parameter navigation. Be sure to add a base tag, href=/: 
		// switch from hash (#) to slash (/) navigation
		config.options.root = "/";
		config.options.hashChange = false;

		var step = new AuthorizeStep;
		config.addAuthorizeStep(step);


		config.mapUnknownRoutes({ redirect: '/' });

		config.map([
			{ route: '', moduleId: 'home', title: 'Home' },
			{ route: 'register', moduleId: 'register', title: 'Register' },
			{ route: 'signin', moduleId: 'sign-in', title: 'Sign In' },
			{ route: 'account', moduleId: 'account', title: 'Account', settings: { auth: true } },
			{ route: 'todo', moduleId: 'todo', title: 'Todo List', settings: { auth: true } }
		]);


		this.router = router;
	}
}

export class AuthorizeStep {
	run(navigationInstruction, next) {
		if (navigationInstruction.getAllInstructions().some(i => i.config.settings.auth)) {
			var isLoggedIn = localStorage.getItem('authenticated');
			if (isLoggedIn !== "true") {
				return next.cancel(new Redirect('signin'));
			}
		}

		return next();
	}
}
