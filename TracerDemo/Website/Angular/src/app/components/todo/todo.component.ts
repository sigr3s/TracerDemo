import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Api } from '../../services/api';
import { AppService } from '../../services/appService';
import { Overlay } from 'ngx-modialog';
import { Modal } from '../../../../node_modules/ngx-modialog/plugins/bootstrap';


@Component({
    selector: 'todo',
    templateUrl: './todo.component.html'
})
export class TodoComponent {
    constructor(public _api: Api, public _appService: AppService, public _modal: Modal)
    {
        this.appService = _appService;
        this.teams = new Array<any>();

        //do any operations which are dependent upon a valid user session.
        this.appService.UserLoaded.subscribe(data => {
            if (data == true)
                this.getTeams();
        });

    }
    model: string;
    teams: Array<any>;
    appService: AppService;

    createTeam() {
        this._api.createTeam(this.model).subscribe(data => {
            this.model = '';
            this.teams.push(data.json());
        });
    }

    completeTodo(item: any) {
        item.completed = item.completed ? false : true;
        this._api.completeTodoById(this.appService.User.id, item.id, item.completed).subscribe(data => { });
    }

    deleteTodo(item: any) {
        let result = this._modal.confirm()
            .size('sm')
            .isBlocking(true)
            .showClose(false)
            .title('Confirm')
            .body('Are you certain you want to delete this item?')
            .okBtn("Yes! Do it.")
            .cancelBtn("No")
            .open().result.then(result =>
            {
                this._api.deleteTodoById(this.appService.User.id, item.id).subscribe(data => {
                    this.teams = this.teams.filter(i => i.id != item.id);
                });
            });

    }

    getTeams() {
        this._api.getTeams().subscribe(data => {
            this.teams = data.json();
        });
    }

}
