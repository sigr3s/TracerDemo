import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';

@Component({
    selector: 'register',
    templateUrl: './register.component.html'
})
export class RegisterComponent {
    constructor(public _api: Api) { }
    model = new Register()
    message: string;
    error: boolean;
    showMessage: boolean;

    register() {
        this.showMessage = false;
        this._api.registerUser(this.model.email, this.model.password, this.model.confirmPassword).subscribe(
            data => {
                this.message = 'Registration Complete. Please Sign In';
                this.error = false;
                this.showMessage = true;
            },
            error => {
                this.message = error.text();
                this.error = true;
                this.showMessage = true;
            });
    }
}

export class Register {
    public email: string;
    public password: string;
    public confirmPassword: string;
}
