import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

@Injectable()
export class AppService {
    constructor() {
        this.UserLoaded = new BehaviorSubject<boolean>(false);
    }

    public User: any;

    //Subscribe to this UserLoaded property to know when the user data has been loaded. Ex: The account and todo pages need to load data when the page loads. If the current app user hasn't loaded when these initial services are requested, an error could be produced.
    public UserLoaded: BehaviorSubject<boolean>;

}

