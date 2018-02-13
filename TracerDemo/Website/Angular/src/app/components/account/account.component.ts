import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { AppService } from '../../services/appService';
import { Overlay } from 'ngx-modialog';
import { Modal } from '../../../../node_modules/ngx-modialog/plugins/bootstrap';
import { Router } from '@angular/router';


@Component({
    selector: 'account',
    templateUrl: './account.component.html'
})
export class AccountComponent {
    constructor(public _api: Api, public _appService: AppService, public _modal: Modal, public _router: Router) {
        this.appService = _appService;

        //do any operations which are dependent upon a valid user session.
        this.appService.UserLoaded.subscribe(data => {
            if (data == true)
                this.getCurrentUser();
        });

    }
    email: string;
    name: string;
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
    appService: AppService;

    saveUserChanges() {
        this._api.updateUserById(this._appService.User.id, this.email, this.name).subscribe(data => {
            this._modal.alert()
                .size('sm')
                .isBlocking(true)
                .showClose(false)
                .title('Update')
                .body('Your account changes have been saved.')
                .okBtn("Thanks")
                .open();

            //Re-fetch the current user data. Exact same process as when the application first loads.
            this._api.getCurrentUser().subscribe(
                data => {
                    this.appService.User = data.json();
                    this.appService.UserLoaded.next(true);
                },
                error => {
                    this._router.navigate(['/signin']);
                });
        },
        error => {
            this._modal.alert()
                .size('sm')
                .isBlocking(true)
                .showClose(false)
                .title('Error')
                .body(error.text())
                .okBtn("Ok")
                .open();
        });
    }

    deleteUser() {
        let result = this._modal.confirm()
            .size('sm')
            .isBlocking(true)
            .showClose(false)
            .title('Confirm')
            .body('Are you certain you want to delete your account?')
            .okBtn("Yes! Do it.")
            .cancelBtn("No")
            .open().result.then(result => {
                this._api.deleteUserById(this._appService.User.id).subscribe(data => {
                    this._api.logoutUser().subscribe(data => {
                        this.appService.User = null;
                        this._router.navigate(['/signin']);
                    })
                });
            });

    }

    changeUserPassword() {
        this._api.changeUserPasswordById(this._appService.User.id, this.currentPassword, this.newPassword, this.confirmPassword).subscribe(data => {
            this._modal.alert()
                .size('sm')
                .isBlocking(true)
                .showClose(false)
                .title('Update')
                .body('Your password has been changed.')
                .okBtn("Thanks")
                .open();
        },
        error => {
            this._modal.alert()
                .size('sm')
                .isBlocking(true)
                .showClose(false)
                .title('Error')
                .body(error.text())
                .okBtn("Ok")
                .open();
        });
    }

    getCurrentUser() {
        this._api.getCurrentUser().subscribe(data => {
            this.name = data.json().name;
            this.email = data.json().email;
        });
    }

}
