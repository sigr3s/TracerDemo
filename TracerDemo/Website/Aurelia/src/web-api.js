import { inject } from 'aurelia-framework';
import { HttpClient, json } from 'aurelia-http-client';

@inject(HttpClient)
export class WebAPI {
	constructor(client) {
		this.client = new HttpClient()
			.configure(x => {
		x.withBaseUrl(document.domain === 'localhost' ? 'http://localhost:58622/api/' : 'https://netmongo.bckbtn.com/api/');
        x.withHeader('Accept', 'application/json');
        x.withCredentials(true);
			});
	}
	isRequesting = false;


	result(success, content) {
		this.isRequesting = false;
		return { success, content };
	}

	registerUser(email, password, confirmPassword) {
		this.isRequesting = true;
		let user = { email, password, confirmPassword };
		return this.client.post('v1/users', user)
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	authenticateUser(email, password) {
		this.isRequesting = true;
		let user = { email, password };
		return this.client.post('v1/authenticate', user)
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	logoutUser() {
		this.isRequesting = true;
		return this.client.get('v1/logout')
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	getCurrentUser() {
		this.isRequesting = true;
		return this.client.get('v1/users/me')
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	deleteUserById(userId)
	{
		this.isRequesting = true;
		return this.client.delete('v1/users/' + userId)
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	updateUserById(userId, email, name)
	{
		this.isRequesting = true;
		let content = { email, name };
		return this.client.put('v1/users/' + userId, content)
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	changeUserPasswordById(userId, oldPassword, newPassword, confirmPassword)
	{
		this.isRequesting = true;
		let content = { oldPassword, newPassword, confirmPassword };
		return this.client.put('v1/users/' + userId + '/password', content)
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	createTodoByUser(userId, task) {
		this.isRequesting = true;
		let content = { task };
		return this.client.post('v1/users/'+ userId + '/todo', content)
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	getTodosByUserId(userId) {
		this.isRequesting = true;
		return this.client.get('v1/users/' + userId + '/todo')
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	completeTodoById(userId, todoId, status) {
		this.isRequesting = true;
		return this.client.put('v1/users/' + userId + '/todo/' + todoId + '/status/' + status)
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
	}

	deleteTodoById(userId, todoId) {
		this.isRequesting = true;
		return this.client.delete('v1/users/' + userId + '/todo/' + todoId)
			.then(response => this.result(true, response.content))
			.catch(response => this.result(false, response.content));
  }
  
}
