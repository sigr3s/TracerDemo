import {WebAPI} from './web-api';
import {inject} from 'aurelia-framework';

@inject(WebAPI)
export class Register {     
  constructor(api) {
    this.api = api;
  }

  register()
  {
    this.api.registerUser(this.email,this.password,this.confirmpassword)
    .then(r => {
      if(r.success === true)
      {
        this.results = 'Registration Complete. Please Sign In';
        this.error = false;
      }
      else
      {
        this.results = r.content;
        this.error = r.success === false;
      }
    });
  }
}
