import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { Api } from '../../services/api';
import { AppService } from '../../services/appService';
import { Router } from '@angular/router';
import { Observable } from 'rxjs/Observable';
import { Subject } from 'rxjs/Subject';

//import ViewEncapsulation and set encapsulation to ViewEncapsulation.None - this enables global css styles
@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    encapsulation: ViewEncapsulation.None
})
export class AppComponent implements OnInit {
    constructor(public _api: Api, public _router: Router, public _appService: AppService) {
        this.appService = _appService;
    }

    ngOnInit(): void {

        this._api.getCurrentUser().subscribe(
            data => {
                this.appService.User = data.json();
                this.appService.UserLoaded.next(true);
            },
            error => {
                this._router.navigate(['/signin']);
            });

    }

    public appService: AppService;

    logout() {
        this._api.logoutUser().subscribe(data => {
            this.appService.User = null;
            this._router.navigate(['/signin']);
        });
    }

}
