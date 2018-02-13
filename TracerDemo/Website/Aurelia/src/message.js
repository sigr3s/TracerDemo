import { DialogController } from 'aurelia-dialog';
import { inject } from 'aurelia-framework';

@inject(DialogController)
export class message {
	constructor(DialogController) {
		this.controller = DialogController;
	}
	model = { message: '', confirm: null, cancel: null };

	activate(model) {
		this.model = model;
	}
}
