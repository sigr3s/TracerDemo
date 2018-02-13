import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { AppService } from '../../services/appService';
import { Router } from '@angular/router';

@Component({
    selector: 'signin',
    templateUrl: './signin.component.html'
})
export class SignInComponent {
    constructor(public _api: Api, public _router: Router, public _appService: AppService)
    {
        this.appService = _appService;
    }
    model = new SignIn()
    message: string;
    error: boolean;
    showMessage: boolean;
    appService: AppService;

    authenticate() {
        this.showMessage = false;
        this._api.authenticateUser(this.model.email, this.model.password).subscribe(
            data => {
                this.error = false;
                this.showMessage = true;
                this.appService.User = data.json();
                this.appService.UserLoaded.next(true);

                this._router.navigate(['/todo']);
            },
            error => {
                this.message = error.text();
                this.error = true;
                this.showMessage = true;
            });
    }
}

export class SignIn {
    public email: string;
    public password: string;
}