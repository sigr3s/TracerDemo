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
    teamName : string;
    summonerName : string;

    createTeam() {
        this._api.createTeam(this.model).subscribe(data => {
            this.model = '';
            this.teams.push(data.json());
        });
    }

    addSummoner(){
      this._api.AddSummoner(this.teamName, this.summonerName).subscribe(data => {
         this.getTeams();
      })
    }

    getTeams() {
        this._api.getTeams().subscribe(data => {
            this.teams = data.json();
        });
    }

}
