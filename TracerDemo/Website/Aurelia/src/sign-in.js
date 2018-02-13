import { WebAPI } from './web-api';
import { AppService } from './app-service';
import { Router } from 'aurelia-router';
import { inject } from 'aurelia-framework';

@inject(WebAPI, Router, AppService)
export class SignIn {     
    constructor(WebAPI, Router, AppService) {
        this.appService = AppService;
        this.api = WebAPI;
        this.router = Router;
  }

  authenticate()
  {
    this.api.authenticateUser(this.email,this.password)
    .then(r => {
      if(r.success === true)
      {
          this.appService.user = r.content;
          localStorage.setItem('authenticated', true);
          this.router.navigate("todo");
      }
      else
      {
        this.results = r.content;
        this.error = r.success === false;
      }
    });
  }
}
